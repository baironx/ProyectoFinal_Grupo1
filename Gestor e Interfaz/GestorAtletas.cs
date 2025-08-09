using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Servicios
{
    /// <summary>
    /// Gestor de atletas que implementa la lógica de negocio.
    /// Implementa el patrón Application Service y cumple con SRP.
    /// </summary>
    public class GestorAtletas : IGestorAtletas
    {
        #region Campos Privados

        private readonly IRepositorioAtletas<Atleta> _repositorio;
        private readonly IValidadorDatos<Atleta> _validador;

        // Delegates para eventos y notificaciones
        public delegate void NotificadorCambio(Atleta atleta, string tipoOperacion);
        public delegate bool ConfirmadorOperacion(Atleta atleta, string operacion);

        private readonly NotificadorCambio _notificador;
        private readonly ConfirmadorOperacion _confirmador;

        #endregion

        #region Eventos

        public event NotificadorCambio AtletaAgregado;
        public event NotificadorCambio AtletaActualizado;
        public event NotificadorCambio AtletaEliminado;

        #endregion

        #region Constructor

        public GestorAtletas(IRepositorioAtletas<Atleta> repositorio, IValidadorDatos<Atleta> validador)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _validador = validador ?? throw new ArgumentNullException(nameof(validador));

            // Configurar delegates por defecto
            _notificador = (atleta, operacion) => Console.WriteLine($"Operación '{operacion}' realizada en atleta {atleta.Nombre}");
            _confirmador = (atleta, operacion) => true; // Por defecto, confirmar siempre
        }

        #endregion

        #region Implementación de IGestorAtletas

        public IEnumerable<Atleta> ObtenerTodos()
        {
            try
            {
                return _repositorio.ObtenerTodos().OrderBy(a => a.Nombre);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener atletas", ex);
            }
        }

        public void Agregar(Atleta atleta)
        {
            if (atleta == null)
                throw new ArgumentNullException(nameof(atleta));

            // Validar antes de agregar
            if (!_validador.Validar(atleta))
            {
                var errores = _validador.ObtenerErrores(atleta);
                throw new ArgumentException($"Atleta inválido: {string.Join(", ", errores)}");
            }

            // Verificar duplicados por nombre
            var atletaExistente = _repositorio.ObtenerTodos()
                .FirstOrDefault(a => a.Nombre.Equals(atleta.Nombre, StringComparison.OrdinalIgnoreCase));

            if (atletaExistente != null)
            {
                throw new InvalidOperationException($"Ya existe un atleta con el nombre '{atleta.Nombre}'");
            }

            // Confirmar operación si es necesario
            if (!_confirmador(atleta, "Agregar"))
            {
                throw new OperationCanceledException("Operación cancelada por el usuario");
            }

            try
            {
                _repositorio.Agregar(atleta);

                // Notificar y disparar evento
                _notificador(atleta, "Agregar");
                AtletaAgregado?.Invoke(atleta, "Agregar");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al agregar atleta", ex);
            }
        }

        public void Actualizar(Atleta atletaOriginal, Atleta atletaActualizado)
        {
            if (atletaOriginal == null)
                throw new ArgumentNullException(nameof(atletaOriginal));
            if (atletaActualizado == null)
                throw new ArgumentNullException(nameof(atletaActualizado));

            // Validar atleta actualizado
            if (!_validador.Validar(atletaActualizado))
            {
                var errores = _validador.ObtenerErrores(atletaActualizado);
                throw new ArgumentException($"Datos actualizados inválidos: {string.Join(", ", errores)}");
            }

            // Verificar que el atleta original existe
            var atletaExistente = _repositorio.ObtenerPorId(atletaOriginal.Id);
            if (atletaExistente == null)
            {
                throw new InvalidOperationException("El atleta a actualizar no existe");
            }

            // Verificar duplicados de nombre (excluyendo el atleta actual)
            if (!atletaOriginal.Nombre.Equals(atletaActualizado.Nombre, StringComparison.OrdinalIgnoreCase))
            {
                var nombreDuplicado = _repositorio.ObtenerTodos()
                    .Any(a => a.Id != atletaOriginal.Id &&
                             a.Nombre.Equals(atletaActualizado.Nombre, StringComparison.OrdinalIgnoreCase));

                if (nombreDuplicado)
                {
                    throw new InvalidOperationException($"Ya existe otro atleta con el nombre '{atletaActualizado.Nombre}'");
                }
            }

            // Confirmar operación
            if (!_confirmador(atletaActualizado, "Actualizar"))
            {
                throw new OperationCanceledException("Actualización cancelada por el usuario");
            }

            try
            {
                // Actualizar usando el método de la entidad
                atletaOriginal.ActualizarCon(atletaActualizado);
                _repositorio.Actualizar(atletaOriginal);

                // Notificar y disparar evento
                _notificador(atletaOriginal, "Actualizar");
                AtletaActualizado?.Invoke(atletaOriginal, "Actualizar");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al actualizar atleta", ex);
            }
        }

        public void Eliminar(Atleta atleta)
        {
            if (atleta == null)
                throw new ArgumentNullException(nameof(atleta));

            // Verificar que el atleta existe
            var atletaExistente = _repositorio.ObtenerPorId(atleta.Id);
            if (atletaExistente == null)
            {
                throw new InvalidOperationException("El atleta a eliminar no existe");
            }

            // Confirmar operación crítica
            if (!_confirmador(atleta, "Eliminar"))
            {
                throw new OperationCanceledException("Eliminación cancelada por el usuario");
            }

            try
            {
                _repositorio.Eliminar(atletaExistente);

                // Notificar y disparar evento
                _notificador(atletaExistente, "Eliminar");
                AtletaEliminado?.Invoke(atletaExistente, "Eliminar");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al eliminar atleta", ex);
            }
        }

        #endregion

        #region Métodos Adicionales

        /// <summary>
        /// Busca atletas usando múltiples criterios con LINQ.
        /// </summary>
        public IEnumerable<Atleta> BuscarPorCriterios(Dictionary<string, object> criterios)
        {
            var query = _repositorio.ObtenerTodos().AsQueryable();

            if (criterios.ContainsKey("nombre") && criterios["nombre"] != null)
            {
                var nombre = criterios["nombre"].ToString();
                query = query.Where(a => a.Nombre.Contains(nombre!, StringComparison.OrdinalIgnoreCase));
            }

            if (criterios.ContainsKey("nivel") && criterios["nivel"] != null)
            {
                var nivel = criterios["nivel"].ToString();
                query = query.Where(a => a.Nivel.Equals(nivel, StringComparison.OrdinalIgnoreCase));
            }

            if (criterios.ContainsKey("objetivos") && criterios["objetivos"] != null)
            {
                var objetivos = criterios["objetivos"].ToString();
                query = query.Where(a => a.Objetivos.Contains(objetivos!, StringComparison.OrdinalIgnoreCase));
            }

            if (criterios.ContainsKey("imc_min") && criterios["imc_min"] != null)
            {
                var imcMin = Convert.ToDouble(criterios["imc_min"]);
                query = query.Where(a => a.CalcularIMC() >= imcMin);
            }

            if (criterios.ContainsKey("imc_max") && criterios["imc_max"] != null)
            {
                var imcMax = Convert.ToDouble(criterios["imc_max"]);
                query = query.Where(a => a.CalcularIMC() <= imcMax);
            }

            return query.ToList();
        }

        /// <summary>
        /// Obtiene estadísticas generales de atletas usando LINQ.
        /// </summary>
        public Dictionary<string, object> ObtenerEstadisticasGenerales()
        {
            var atletas = _repositorio.ObtenerTodos().ToList();

            if (!atletas.Any())
                return new Dictionary<string, object> { ["mensaje"] = "No hay atletas registrados" };

            return new Dictionary<string, object>
            {
                ["TotalAtletas"] = atletas.Count,
                ["PesoPromedio"] = atletas.Average(a => a.Peso),
                ["AlturaPromedio"] = atletas.Average(a => a.Altura),
                ["IMCPromedio"] = atletas.Average(a => a.CalcularIMC()),
                ["AtletasPorNivel"] = atletas.GroupBy(a => a.Nivel)
                                           .ToDictionary(g => g.Key, g => g.Count()),
                ["ObjetivosMasFrecuentes"] = atletas.GroupBy(a => a.Objetivos)
                                                  .OrderByDescending(g => g.Count())
                                                  .Take(3)
                                                  .ToDictionary(g => g.Key, g => g.Count()),
                ["RangoIMC"] = new
                {
                    Minimo = atletas.Min(a => a.CalcularIMC()),
                    Maximo = atletas.Max(a => a.CalcularIMC())
                }
            };
        }

        /// <summary>
        /// Valida la compatibilidad de un atleta con un tipo de entrenamiento.
        /// </summary>
        public (bool EsCompatible, List<string> Recomendaciones) EvaluarCompatibilidad(Atleta atleta, string tipoEntrenamiento)
        {
            var recomendaciones = new List<string>();
            var esCompatible = true;

            if (atleta == null)
                return (false, new List<string> { "Atleta no válido" });

            var imc = atleta.CalcularIMC();
            var nivel = atleta.Nivel.ToLower();

            // Evaluación por IMC
            if (imc > 30 && tipoEntrenamiento.ToLower() == "fuerza")
            {
                recomendaciones.Add("Considerar comenzar con cardio para reducir peso antes de entrenamiento intenso de fuerza");
            }

            if (imc < 18.5 && tipoEntrenamiento.ToLower() == "cardio")
            {
                recomendaciones.Add("Considerar agregar entrenamiento de fuerza para ganancia de masa muscular");
            }

            // Evaluación por nivel
            if (nivel == "principiante" && tipoEntrenamiento.ToLower().Contains("alta intensidad"))
            {
                esCompatible = false;
                recomendaciones.Add("Comenzar con entrenamientos de baja intensidad y progresar gradualmente");
            }

            // Evaluación por objetivos
            if (!atleta.EsCompatibleCon(tipoEntrenamiento))
            {
                recomendaciones.Add($"El entrenamiento {tipoEntrenamiento} no está alineado con los objetivos actuales");
            }

            if (!recomendaciones.Any())
            {
                recomendaciones.Add("Atleta compatible con el tipo de entrenamiento seleccionado");
            }

            return (esCompatible, recomendaciones);
        }

        #endregion

    }
}
