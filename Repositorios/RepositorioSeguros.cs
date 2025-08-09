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
    /// Implementación de repositorio de seguros médicos usando archivos.
    /// </summary>
    public class RepositorioSegurosArchivo<T> : IRepositorioSeguros<T> where T : class
    {
        #region Campos Privados

        private readonly string _rutaArchivo;
        private readonly List<T> _seguros;
        private readonly object _lockObject = new object();

        // Delegates para búsquedas especializadas
        public delegate bool PredicadoBusqueda(T seguro, object criterio);
        public delegate decimal CalculadorMonto(T seguro);

        private readonly Dictionary<string, PredicadoBusqueda> _predicados;
        private readonly CalculadorMonto _calculadorMonto;

        #endregion

        #region Constructor

        public RepositorioSegurosArchivo(string rutaArchivo)
        {
            _rutaArchivo = rutaArchivo ?? throw new ArgumentNullException(nameof(rutaArchivo));
            _seguros = new List<T>();

            // Configurar predicados de búsqueda
            _predicados = new Dictionary<string, PredicadoBusqueda>
            {
                ["nombre"] = (seguro, criterio) => (seguro as SeguroMedico)?.NombreSeguro.Contains(criterio.ToString()!, StringComparison.OrdinalIgnoreCase) == true,
                ["atleta"] = (seguro, criterio) => (seguro as SeguroMedico)?.NombreAtleta.Equals(criterio.ToString(), StringComparison.OrdinalIgnoreCase) == true,
                ["lesion"] = (seguro, criterio) => (seguro as SeguroMedico)?.LesionTratada.Contains(criterio.ToString()!, StringComparison.OrdinalIgnoreCase) == true,
                ["estado"] = (seguro, criterio) => (seguro as SeguroMedico)?.Estado.ToString().Equals(criterio.ToString(), StringComparison.OrdinalIgnoreCase) == true
            };

            // Calculador de monto total
            _calculadorMonto = (seguro) => (seguro as SeguroMedico)?.CalcularMontoTotal() ?? 0;

            CargarDatos();
        }

        #endregion

        #region Implementación de IRepositorio<T>

        public IEnumerable<T> ObtenerTodos()
        {
            lock (_lockObject)
            {
                return _seguros.OrderByDescending(s => (s as SeguroMedico)?.FechaAplicacion).ToList();
            }
        }

        public T ObtenerPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null!;

            lock (_lockObject)
            {
                return _seguros.FirstOrDefault(s => (s as SeguroMedico)?.Id == id)!;
            }
        }

        public void Agregar(T seguro)
        {
            if (seguro == null)
                throw new ArgumentNullException(nameof(seguro));

            if (seguro is IValidable validable && !validable.EsValido())
                throw new ArgumentException("El seguro médico no es válido");

            lock (_lockObject)
            {
                _seguros.Add(seguro);
                GuardarCambios();
            }
        }

        public void Actualizar(T seguro)
        {
            if (seguro == null)
                throw new ArgumentNullException(nameof(seguro));

            var id = (seguro as SeguroMedico)?.Id;
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("El seguro debe tener un ID válido");

            lock (_lockObject)
            {
                var indice = _seguros.FindIndex(s => (s as SeguroMedico)?.Id == id);
                if (indice >= 0)
                {
                    _seguros[indice] = seguro;
                    GuardarCambios();
                }
                else
                {
                    throw new InvalidOperationException("Seguro no encontrado para actualizar");
                }
            }
        }

        public void Eliminar(T seguro)
        {
            if (seguro == null)
                throw new ArgumentNullException(nameof(seguro));

            lock (_lockObject)
            {
                if (_seguros.Remove(seguro))
                {
                    GuardarCambios();
                }
                else
                {
                    throw new InvalidOperationException("Seguro no encontrado para eliminar");
                }
            }
        }

        public void GuardarCambios()
        {
            try
            {
                var datosSerializados = _seguros.Select(s => JsonSerializer.Serialize(s));
                File.WriteAllLines(_rutaArchivo, datosSerializados);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar seguros: {ex.Message}", ex);
            }
        }

        #endregion

        #region Implementación de IRepositorioSeguros<T>

        public IEnumerable<T> BuscarPorNombreSeguro(string nombreSeguro)
        {
            return BuscarPorCriterio("nombre", nombreSeguro);
        }

        public IEnumerable<T> BuscarPorMontoCubierto(decimal monto)
        {
            lock (_lockObject)
            {
                return _seguros.Where(s => (s as SeguroMedico)?.MontoCubierto == monto).ToList();
            }
        }

        public IEnumerable<T> BuscarPorMontoPaciente(decimal monto)
        {
            lock (_lockObject)
            {
                return _seguros.Where(s => (s as SeguroMedico)?.MontoPaciente == monto).ToList();
            }
        }

        public IEnumerable<T> BuscarPorAtleta(string nombreAtleta)
        {
            return BuscarPorCriterio("atleta", nombreAtleta);
        }

        #endregion

        #region Métodos Adicionales con LINQ y Delegates

        /// <summary>
        /// Busca seguros por criterio usando delegates.
        /// </summary>
        private IEnumerable<T> BuscarPorCriterio(string tipoCriterio, object valor)
        {
            if (string.IsNullOrWhiteSpace(valor?.ToString()))
                return Enumerable.Empty<T>();

            if (!_predicados.ContainsKey(tipoCriterio))
                return Enumerable.Empty<T>();

            lock (_lockObject)
            {
                var predicado = _predicados[tipoCriterio];
                return _seguros.Where(s => predicado(s, valor)).ToList();
            }
        }

        /// <summary>
        /// Obtiene seguros por rango de montos usando LINQ.
        /// </summary>
        public IEnumerable<T> BuscarPorRangoMontos(decimal montoMinimo, decimal montoMaximo)
        {
            lock (_lockObject)
            {
                return _seguros.Where(s =>
                {
                    var montoTotal = _calculadorMonto(s);
                    return montoTotal >= montoMinimo && montoTotal <= montoMaximo;
                }).ToList();
            }
        }

        /// <summary>
        /// Obtiene seguros activos usando LINQ.
        /// </summary>
        public IEnumerable<T> ObtenerSegurosActivos()
        {
            return BuscarPorCriterio("estado", "Activo");
        }

        /// <summary>
        /// Obtiene análisis financiero de seguros.
        /// </summary>
        public Dictionary<string, decimal> ObtenerAnalisisFinanciero(string nombreAtleta = null!)
        {
            lock (_lockObject)
            {
                var segurosConsulta = _seguros.AsEnumerable();

                if (!string.IsNullOrEmpty(nombreAtleta))
                {
                    var predicado = _predicados["atleta"];
                    segurosConsulta = segurosConsulta.Where(s => predicado(s, nombreAtleta));
                }

                var segurosSeguro = segurosConsulta.Cast<SeguroMedico>().ToList();

                if (!segurosSeguro.Any())
                    return new Dictionary<string, decimal>();

                return new Dictionary<string, decimal>
                {
                    ["TotalMontoCubierto"] = segurosSeguro.Sum(s => s.MontoCubierto),
                    ["TotalMontoPaciente"] = segurosSeguro.Sum(s => s.MontoPaciente),
                    ["MontoTotal"] = segurosSeguro.Sum(s => s.CalcularMontoTotal()),
                    ["PromedioMontoCubierto"] = segurosSeguro.Average(s => s.MontoCubierto),
                    ["PromedioMontoPaciente"] = segurosSeguro.Average(s => s.MontoPaciente),
                    ["MontoMaximo"] = segurosSeguro.Max(s => s.CalcularMontoTotal()),
                    ["MontoMinimo"] = segurosSeguro.Min(s => s.CalcularMontoTotal())
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
                                var seguro = JsonSerializer.Deserialize<T>(linea);
                                if (seguro != null)
                                {
                                    _seguros.Add(seguro);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error al cargar seguro: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al cargar seguros desde {_rutaArchivo}: {ex.Message}", ex);
            }
        }

        #endregion
    }
}