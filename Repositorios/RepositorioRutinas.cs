using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Repositorios
{
    /// <summary>
    /// Implementación de repositorio de rutinas usando archivos como almacenamiento.
    /// </summary>
    public class RepositorioRutinasArchivo<T> : IRepositorioRutinas<T> where T : class
    {
        #region Campos Privados

        private readonly string _rutaArchivo;
        private readonly List<T> _rutinas;
        private readonly object _lockObject = new object();

        // Delegates para filtros y transformaciones
        public delegate bool FiltroRutina(T rutina, object criterio);
        public delegate IEnumerable<T> TransformadorResultados(IEnumerable<T> resultados);

        private readonly Dictionary<string, FiltroRutina> _filtros;
        private readonly TransformadorResultados _transformador;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del repositorio de rutinas.
        /// </summary>
        public RepositorioRutinasArchivo(string rutaArchivo)
        {
            _rutaArchivo = rutaArchivo ?? throw new ArgumentNullException(nameof(rutaArchivo));
            _rutinas = new List<T>();

            // Configurar filtros con delegates
            _filtros = new Dictionary<string, FiltroRutina>
            {
                ["tipo"] = (rutina, criterio) => (rutina as Rutina)?.Tipo.Equals(criterio.ToString(), StringComparison.OrdinalIgnoreCase) == true,
                ["intensidad"] = (rutina, criterio) => (rutina as Rutina)?.Intensidad.Equals(criterio.ToString(), StringComparison.OrdinalIgnoreCase) == true,
                ["atleta"] = (rutina, criterio) => (rutina as Rutina)?.NombreAtleta.Equals(criterio.ToString(), StringComparison.OrdinalIgnoreCase) == true,
                ["grupo"] = (rutina, criterio) => (rutina as Rutina)?.GrupoMuscular.Contains(criterio.ToString()!, StringComparison.OrdinalIgnoreCase) == true
            };

            // Transformador por defecto (ordenar por fecha)
            _transformador = (resultados) => resultados.OrderByDescending(r => (r as Rutina)?.FechaRealizacion);

            CargarDatos();
        }

        #endregion

        #region Implementación de IRepositorio<T>

        public IEnumerable<T> ObtenerTodos()
        {
            lock (_lockObject)
            {
                return _transformador(_rutinas).ToList();
            }
        }

        public T ObtenerPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null!;

            lock (_lockObject)
            {
                return _rutinas.FirstOrDefault(r => (r as Rutina)?.Id == id)!;
            }
        }

        public void Agregar(T rutina)
        {
            if (rutina == null)
                throw new ArgumentNullException(nameof(rutina));

            if (rutina is IValidable validable && !validable.EsValido())
                throw new ArgumentException("La rutina no es válida");

            lock (_lockObject)
            {
                _rutinas.Add(rutina);
                GuardarCambios();
            }
        }

        public void Actualizar(T rutina)
        {
            if (rutina == null)
                throw new ArgumentNullException(nameof(rutina));

            var id = (rutina as Rutina)?.Id;
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("La rutina debe tener un ID válido");

            lock (_lockObject)
            {
                var indice = _rutinas.FindIndex(r => (r as Rutina)?.Id == id);
                if (indice >= 0)
                {
                    _rutinas[indice] = rutina;
                    GuardarCambios();
                }
                else
                {
                    throw new InvalidOperationException("Rutina no encontrada para actualizar");
                }
            }
        }

        public void Eliminar(T rutina)
        {
            if (rutina == null)
                throw new ArgumentNullException(nameof(rutina));

            lock (_lockObject)
            {
                if (_rutinas.Remove(rutina))
                {
                    GuardarCambios();
                }
                else
                {
                    throw new InvalidOperationException("Rutina no encontrada para eliminar");
                }
            }
        }

        public void GuardarCambios()
        {
            try
            {
                var datosSerializados = _rutinas.Select(r => JsonSerializer.Serialize(r, r.GetType()));
                File.WriteAllLines(_rutaArchivo, datosSerializados);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar rutinas: {ex.Message}", ex);
            }
        }

        #endregion

        #region Implementación de IRepositorioRutinas<T>

        public IEnumerable<T> ObtenerPorAtleta(string nombreAtleta)
        {
            if (string.IsNullOrWhiteSpace(nombreAtleta))
                return Enumerable.Empty<T>();

            lock (_lockObject)
            {
                var filtro = _filtros["atleta"];
                return _transformador(_rutinas.Where(r => filtro(r, nombreAtleta))).ToList();
            }
        }

        public IEnumerable<T> BuscarPorTipo(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return Enumerable.Empty<T>();

            lock (_lockObject)
            {
                var filtro = _filtros["tipo"];
                return _transformador(_rutinas.Where(r => filtro(r, tipo))).ToList();
            }
        }

        public IEnumerable<T> BuscarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            lock (_lockObject)
            {
                return _transformador(_rutinas.Where(r =>
                {
                    var fecha = (r as Rutina)?.FechaRealizacion;
                    return fecha >= fechaInicio && fecha <= fechaFin;
                })).ToList();
            }
        }

        public IEnumerable<T> BuscarConLesiones()
        {
            lock (_lockObject)
            {
                return _transformador(_rutinas.Where(r =>
                    !string.IsNullOrEmpty((r as Rutina)?.LesionesPostEntrenamiento))).ToList();
            }
        }

        #endregion

        #region Métodos Adicionales con LINQ

        /// <summary>
        /// Busca rutinas usando múltiples criterios con LINQ.
        /// </summary>
        public IEnumerable<T> BuscarPorCriterios(Dictionary<string, object> criterios)
        {
            if (criterios == null || !criterios.Any())
                return ObtenerTodos();

            lock (_lockObject)
            {
                var resultados = _rutinas.AsEnumerable();

                // Aplicar filtros dinámicamente usando LINQ
                foreach (var criterio in criterios)
                {
                    if (_filtros.ContainsKey(criterio.Key) && criterio.Value != null)
                    {
                        var filtro = _filtros[criterio.Key];
                        resultados = resultados.Where(r => filtro(r, criterio.Value));
                    }
                }

                return _transformador(resultados).ToList();
            }
        }

        /// <summary>
        /// Obtiene estadísticas de rutinas usando LINQ.
        /// </summary>
        public Dictionary<string, object> ObtenerEstadisticas(string nombreAtleta = null!)
        {
            lock (_lockObject)
            {
                var rutinasConsulta = _rutinas.AsEnumerable();

                if (!string.IsNullOrEmpty(nombreAtleta))
                {
                    rutinasConsulta = rutinasConsulta.Where(r =>
                        (r as Rutina)?.NombreAtleta.Equals(nombreAtleta, StringComparison.OrdinalIgnoreCase) == true);
                }

                var rutinasRutina = rutinasConsulta.Cast<Rutina>().ToList();

                if (!rutinasRutina.Any())
                    return new Dictionary<string, object>();

                return new Dictionary<string, object>
                {
                    ["TotalRutinas"] = rutinasRutina.Count,
                    ["DuracionPromedio"] = rutinasRutina.Average(r => r.Duracion),
                    ["RutinasPorTipo"] = rutinasRutina.GroupBy(r => r.Tipo)
                                                    .ToDictionary(g => g.Key, g => g.Count()),
                    ["RutinasPorIntensidad"] = rutinasRutina.GroupBy(r => r.Intensidad)
                                                          .ToDictionary(g => g.Key, g => g.Count()),
                    ["TotalLesiones"] = rutinasRutina.Count(r => !string.IsNullOrEmpty(r.LesionesPostEntrenamiento)),
                    ["RutinasUltimoMes"] = rutinasRutina.Count(r => r.FechaRealizacion >= DateTime.Today.AddMonths(-1)),
                    ["FechaUltimaRutina"] = rutinasRutina.Max(r => r.FechaRealizacion),
                    ["FechaPrimeraRutina"] = rutinasRutina.Min(r => r.FechaRealizacion)
                };
            }
        }

        #endregion

        #region Métodos Privados

        private void CargarDatos()
        {
            try
            {
                if (File.Exists(_rutaArchivo))
                {
                    var lineas = File.ReadAllLines(_rutaArchivo);
                    foreach (var linea in lineas)
                    {
                        if (!string.IsNullOrWhiteSpace(linea))
                        {
                            try
                            {
                                // Deserialización polimórfica basada en el tipo
                                var rutina = DeserializarRutina(linea);
                                if (rutina != null)
                                {
                                    _rutinas.Add(rutina);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error al cargar rutina: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al cargar rutinas desde {_rutaArchivo}: {ex.Message}", ex);
            }
        }

        private T DeserializarRutina(string datos)
        {
            // Implementación simplificada - en un caso real usaríamos un deserializador más sofisticado
            try
            {
                var doc = JsonDocument.Parse(datos);
                var tipo = doc.RootElement.GetProperty("Tipo").GetString();

                return tipo switch
                {
                    "Fuerza" => JsonSerializer.Deserialize<RutinaFuerza>(datos) as T,
                    "Cardio" => JsonSerializer.Deserialize<RutinaCardio>(datos) as T,
                    _ => JsonSerializer.Deserialize<T>(datos)
                };
            }
            catch
            {
                return JsonSerializer.Deserialize<T>(datos)!;
            }
        }

        #endregion
    }
}
