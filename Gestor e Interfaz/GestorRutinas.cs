using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Servicios
{
    /// <summary>
    /// Gestor de rutinas que implementa la lógica de negocio.
    /// </summary>
    public class GestorRutinas : IGestorRutinas
    {
        #region Campos Privados

        private readonly IRepositorioRutinas<Rutina> _repositorio;
        private readonly IValidadorDatos<Rutina> _validador;

        // Delegates para filtros especializados
        public delegate bool FiltroEspecializado(Rutina rutina, object criterio);

        private readonly Dictionary<string, FiltroEspecializado> _filtros;

        #endregion

        #region Constructor

        public GestorRutinas(IRepositorioRutinas<Rutina> repositorio, IValidadorDatos<Rutina> validador)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _validador = validador ?? throw new ArgumentNullException(nameof(validador));

            // Inicializar filtros con delegates
            _filtros = new Dictionary<string, FiltroEspecializado>
            {
                ["vencimiento"] = (rutina, criterio) => rutina.FechaVencimiento.HasValue &&
                                                       rutina.FechaVencimiento.Value <= (DateTime)criterio,
                ["calorias"] = (rutina, criterio) => rutina.CalcularCaloriasQuemadas() >= (double)criterio,
                ["lesiones"] = (rutina, criterio) => !string.IsNullOrEmpty(rutina.LesionesPostEntrenamiento)
            };
        }

        #endregion

        #region Implementación de IGestorRutinas

        public IEnumerable<Rutina> ObtenerTodos()
        {
            return _repositorio.ObtenerTodos().OrderByDescending(r => r.FechaRealizacion);
        }

        public IEnumerable<Rutina> ObtenerPorAtleta(string nombreAtleta)
        {
            if (string.IsNullOrWhiteSpace(nombreAtleta))
                return Enumerable.Empty<Rutina>();

            return _repositorio.ObtenerPorAtleta(nombreAtleta);
        }

        public void Agregar(Rutina rutina)
        {
            if (rutina == null)
                throw new ArgumentNullException(nameof(rutina));

            if (!_validador.Validar(rutina))
            {
                var errores = _validador.ObtenerErrores(rutina);
                throw new ArgumentException($"Rutina inválida: {string.Join(", ", errores)}");
            }

            _repositorio.Agregar(rutina);
        }

        public void Actualizar(Rutina rutinaOriginal, Rutina rutinaActualizada)
        {
            if (rutinaOriginal == null)
                throw new ArgumentNullException(nameof(rutinaOriginal));
            if (rutinaActualizada == null)
                throw new ArgumentNullException(nameof(rutinaActualizada));

            if (!_validador.Validar(rutinaActualizada))
            {
                var errores = _validador.ObtenerErrores(rutinaActualizada);
                throw new ArgumentException($"Datos actualizados inválidos: {string.Join(", ", errores)}");
            }

            rutinaOriginal.ActualizarCon(rutinaActualizada);
            _repositorio.Actualizar(rutinaOriginal);
        }

        public void Eliminar(Rutina rutina)
        {
            if (rutina == null)
                throw new ArgumentNullException(nameof(rutina));

            _repositorio.Eliminar(rutina);
        }

        public IEnumerable<Rutina> BuscarRutinas(string nombreAtleta, string termino)
        {
            var rutinas = _repositorio.ObtenerPorAtleta(nombreAtleta);

            if (string.IsNullOrWhiteSpace(termino))
                return rutinas;

            return rutinas.Where(r => r.CoincideCon(termino));
        }

        public IEnumerable<Rutina> BuscarPorRangoFechas(string nombreAtleta, DateTime fechaInicio, DateTime fechaFin)
        {
            return _repositorio.ObtenerPorAtleta(nombreAtleta)
                              .Where(r => r.FechaRealizacion >= fechaInicio && r.FechaRealizacion <= fechaFin);
        }

        public IEnumerable<Rutina> BuscarPorIntensidad(string nombreAtleta, string intensidad)
        {
            return _repositorio.ObtenerPorAtleta(nombreAtleta)
                              .Where(r => r.Intensidad.Equals(intensidad, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Rutina> BusquedaCombinada(string nombreAtleta, string tipo = null!,
                                                    string intensidad = null!, string grupoMuscular = null!)
        {
            var query = _repositorio.ObtenerPorAtleta(nombreAtleta).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(r => r.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(intensidad))
                query = query.Where(r => r.Intensidad.Equals(intensidad, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(grupoMuscular))
                query = query.Where(r => r.GrupoMuscular.Contains(grupoMuscular, StringComparison.OrdinalIgnoreCase));

            return query.ToList();
        }

        #endregion

        #region Métodos Adicionales

        /// <summary>
        /// Obtiene rutinas que requieren atención especial.
        /// </summary>
        public IEnumerable<Rutina> ObtenerRutinasAtencionEspecial(string nombreAtleta)
        {
            var rutinas = _repositorio.ObtenerPorAtleta(nombreAtleta);
            var fechaLimite = DateTime.Today.AddDays(7); // Próxima semana

            return rutinas.Where(r =>
                _filtros["vencimiento"](r, fechaLimite) ||
                _filtros["lesiones"](r, null!));
        }

        #endregion
    }
}