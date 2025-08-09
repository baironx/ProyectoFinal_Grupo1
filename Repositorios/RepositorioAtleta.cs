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
    /// Implementación de repositorio de atletas usando archivos como almacenamiento.
    /// Implementa el patrón Repository y cumple con Single Responsibility Principle (SRP).
    /// </summary>
    public class RepositorioAtletasArchivo<T> : IRepositorioAtletas<T> where T : class
    {
        #region Campos Privados

        private readonly string _rutaArchivo;
        private readonly List<T> _atletas;
        private readonly object _lockObject = new object();

        // Delegates para serialización personalizada
        public delegate string SerializadorAtleta(T atleta);
        public delegate T DeserializadorAtleta(string datos);

        private readonly SerializadorAtleta _serializador;
        private readonly DeserializadorAtleta _deserializador;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del repositorio de atletas.
        /// </summary>
        public RepositorioAtletasArchivo(string rutaArchivo)
        {
            _rutaArchivo = rutaArchivo ?? throw new ArgumentNullException(nameof(rutaArchivo));
            _atletas = new List<T>();

            // Configurar delegates de serialización
            _serializador = (atleta) => JsonSerializer.Serialize(atleta);
            _deserializador = (datos) => JsonSerializer.Deserialize<T>(datos)!;

            CargarDatos();
        }

        #endregion

        #region Implementación de IRepositorio<T>

        /// <summary>
        /// Obtiene todos los atletas.
        /// </summary>
        public IEnumerable<T> ObtenerTodos()
        {
            lock (_lockObject)
            {
                return _atletas.ToList(); // Retorna copia para evitar problemas de concurrencia
            }
        }

        /// <summary>
        /// Obtiene un atleta por su identificador.
        /// </summary>
        public T ObtenerPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null!;

            lock (_lockObject)
            {
                return _atletas.FirstOrDefault(a => (a as Atleta)?.Id == id)!;
            }
        }

        /// <summary>
        /// Agrega un nuevo atleta.
        /// </summary>
        public void Agregar(T atleta)
        {
            if (atleta == null)
                throw new ArgumentNullException(nameof(atleta));

            if (atleta is IValidable validable && !validable.EsValido())
                throw new ArgumentException("El atleta no es válido");

            lock (_lockObject)
            {
                _atletas.Add(atleta);
                GuardarCambios();
            }
        }

        /// <summary>
        /// Actualiza un atleta existente.
        /// </summary>
        public void Actualizar(T atleta)
        {
            if (atleta == null)
                throw new ArgumentNullException(nameof(atleta));

            var id = (atleta as Atleta)?.Id;
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("El atleta debe tener un ID válido");

            lock (_lockObject)
            {
                var indice = _atletas.FindIndex(a => (a as Atleta)?.Id == id);
                if (indice >= 0)
                {
                    _atletas[indice] = atleta;
                    GuardarCambios();
                }
                else
                {
                    throw new InvalidOperationException("Atleta no encontrado para actualizar");
                }
            }
        }

        /// <summary>
        /// Elimina un atleta.
        /// </summary>
        public void Eliminar(T atleta)
        {
            if (atleta == null)
                throw new ArgumentNullException(nameof(atleta));

            lock (_lockObject)
            {
                if (_atletas.Remove(atleta))
                {
                    GuardarCambios();
                }
                else
                {
                    throw new InvalidOperationException("Atleta no encontrado para eliminar");
                }
            }
        }

        /// <summary>
        /// Guarda los cambios en el archivo.
        /// </summary>
        public void GuardarCambios()
        {
            try
            {
                var datosSerializados = _atletas.Select(a => _serializador(a));
                File.WriteAllLines(_rutaArchivo, datosSerializados);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar atletas: {ex.Message}", ex);
            }
        }

        #endregion

        #region Implementación de IRepositorioAtletas<T>

        /// <summary>
        /// Busca atletas por nombre.
        /// </summary>
        public IEnumerable<T> BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Enumerable.Empty<T>();

            lock (_lockObject)
            {
                return _atletas.Where(a => (a as IBuscable)?.CoincideCon(nombre) == true ||
                                          (a as Atleta)?.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase) == true)
                              .ToList();
            }
        }

        /// <summary>
        /// Busca atletas por nivel.
        /// </summary>
        public IEnumerable<T> BuscarPorNivel(string nivel)
        {
            if (string.IsNullOrWhiteSpace(nivel))
                return Enumerable.Empty<T>();

            lock (_lockObject)
            {
                return _atletas.Where(a => (a as Atleta)?.Nivel.Equals(nivel, StringComparison.OrdinalIgnoreCase) == true)
                              .ToList();
            }
        }

        /// <summary>
        /// Busca atletas por objetivos.
        /// </summary>
        public IEnumerable<T> BuscarPorObjetivos(string objetivos)
        {
            if (string.IsNullOrWhiteSpace(objetivos))
                return Enumerable.Empty<T>();

            lock (_lockObject)
            {
                return _atletas.Where(a => (a as Atleta)?.Objetivos.Contains(objetivos, StringComparison.OrdinalIgnoreCase) == true)
                              .ToList();
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Carga los datos desde el archivo.
        /// </summary>
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
                                var atleta = _deserializador(linea);
                                if (atleta != null)
                                {
                                    _atletas.Add(atleta);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error but continue loading other records
                                Console.WriteLine($"Error al cargar atleta: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al cargar atletas desde {_rutaArchivo}: {ex.Message}", ex);
            }
        }

        #endregion
    }
}