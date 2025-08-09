using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Entidades
{
    /// <summary>
    /// Clase base abstracta para rutinas de entrenamiento.
    /// Implementa el patrón Template Method y múltiples interfaces.
    /// </summary>
    public abstract class Rutina : IDescribible, IValidable, ICalculableTiempo, IBuscable, IEditable<Rutina>
    {
        #region Propiedades

        public string Id { get; private set; }
        public abstract string Tipo { get; }
        public int Duracion { get; protected set; }
        public string Intensidad { get; protected set; }
        public string GrupoMuscular { get; protected set; }
        public string NombreAtleta { get; protected set; }
        public DateTime FechaRealizacion { get; protected set; }
        public DateTime? FechaVencimiento { get; protected set; }
        public string LesionesPostEntrenamiento { get; protected set; }
        public SeguroMedico SeguroAplicado { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor base para rutinas.
        /// </summary>
        protected Rutina(int duracion, string intensidad, string grupoMuscular, string nombreAtleta,
                        DateTime fechaRealizacion, DateTime? fechaVencimiento, string lesionesPostEntrenamiento, SeguroMedico seguroAplicado = null!)
        {
            Id = Guid.NewGuid().ToString();
            Duracion = duracion;
            Intensidad = intensidad ?? throw new ArgumentNullException(nameof(intensidad));
            GrupoMuscular = grupoMuscular ?? throw new ArgumentNullException(nameof(grupoMuscular));
            NombreAtleta = nombreAtleta ?? throw new ArgumentNullException(nameof(nombreAtleta));
            FechaRealizacion = fechaRealizacion;
            FechaVencimiento = fechaVencimiento;
            LesionesPostEntrenamiento = lesionesPostEntrenamiento ?? string.Empty;
            SeguroAplicado = seguroAplicado;

            if (!EsValido())
            {
                throw new ArgumentException($"Datos de rutina inválidos: {string.Join(", ", ObtenerErroresValidacion())}");
            }
        }

        #endregion

        #region Métodos Abstractos

        /// <summary>
        /// Método abstracto para describir rutinas específicas.
        /// </summary>
        public abstract string Describir();

        /// <summary>
        /// Método abstracto para obtener características específicas de la rutina.
        /// </summary>
        public abstract string ObtenerCaracteristicasEspecificas();

        #endregion

        #region Métodos Virtuales

        /// <summary>
        /// Calcula las calorías quemadas aproximadas. Puede ser sobrescrito por clases derivadas.
        /// </summary>
        public virtual double CalcularCaloriasQuemadas()
        {
            // Cálculo base según duración e intensidad
            double factor = Intensidad.ToLower() switch
            {
                "baja" => 0.8,
                "media" => 1.2,
                "alta" => 1.8,
                _ => 1.0
            };

            return Duracion * factor * 5; // Base: 5 calorías por minuto
        }

        /// <summary>
        /// Determina si la rutina necesita equipo especial.
        /// </summary>
        /// <returns>True si necesita equipo especial.</returns>
        public virtual bool NecesitaEquipoEspecial()
        {
            return GrupoMuscular.ToLower() switch
            {
                "pecho" or "espalda" => true,
                "piernas" when Intensidad.ToLower() == "alta" => true,
                _ => false
            };
        }

        #endregion

        #region Implementación de Interfaces

        /// <summary>
        /// Implementación de IValidable.
        /// </summary>
        public virtual bool EsValido()
        {
            return !ObtenerErroresValidacion().Any();
        }

        /// <summary>
        /// Implementación de IValidable.
        /// </summary>
        public virtual IEnumerable<string> ObtenerErroresValidacion()
        {
            var errores = new List<string>();

            if (Duracion <= 0 || Duracion > 480) // Máximo 8 horas
                errores.Add("La duración debe estar entre 1 y 480 minutos");

            if (string.IsNullOrWhiteSpace(Intensidad))
                errores.Add("La intensidad es requerida");

            var intensidadesValidas = new[] { "Baja", "Media", "Alta" };
            if (!intensidadesValidas.Contains(Intensidad, StringComparer.OrdinalIgnoreCase))
                errores.Add("La intensidad debe ser Baja, Media o Alta");

            if (string.IsNullOrWhiteSpace(GrupoMuscular))
                errores.Add("El grupo muscular es requerido");

            if (string.IsNullOrWhiteSpace(NombreAtleta))
                errores.Add("El nombre del atleta es requerido");

            if (FechaRealizacion > DateTime.Today)
                errores.Add("La fecha de realización no puede ser futura");

            if (FechaVencimiento.HasValue && FechaVencimiento.Value <= FechaRealizacion)
                errores.Add("La fecha de vencimiento debe ser posterior a la fecha de realización");

            return errores;
        }

        /// <summary>
        /// Implementación de ICalculableTiempo.
        /// </summary>
        public bool EstaVencido()
        {
            return FechaVencimiento.HasValue && FechaVencimiento.Value < DateTime.Today;
        }

        /// <summary>
        /// Implementación de ICalculableTiempo.
        /// </summary>
        public int DiasRestantes()
        {
            if (!FechaVencimiento.HasValue) return int.MaxValue;
            return (FechaVencimiento.Value - DateTime.Today).Days;
        }

        /// <summary>
        /// Implementación de IBuscable.
        /// </summary>
        public virtual bool CoincideCon(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino)) return false;

            var terminoLower = termino.ToLower();
            return Tipo.ToLower().Contains(terminoLower) ||
                   Intensidad.ToLower().Contains(terminoLower) ||
                   GrupoMuscular.ToLower().Contains(terminoLower) ||
                   NombreAtleta.ToLower().Contains(terminoLower) ||
                   (!string.IsNullOrEmpty(LesionesPostEntrenamiento) && LesionesPostEntrenamiento.ToLower().Contains(terminoLower));
        }

        /// <summary>
        /// Implementación de IEditable.
        /// </summary>
        public virtual void ActualizarCon(Rutina nuevaRutina)
        {
            if (nuevaRutina == null)
                throw new ArgumentNullException(nameof(nuevaRutina));

            if (!nuevaRutina.EsValido())
                throw new ArgumentException("Los nuevos datos de la rutina son inválidos");

            Duracion = nuevaRutina.Duracion;
            Intensidad = nuevaRutina.Intensidad;
            GrupoMuscular = nuevaRutina.GrupoMuscular;
            FechaRealizacion = nuevaRutina.FechaRealizacion;
            FechaVencimiento = nuevaRutina.FechaVencimiento;
            LesionesPostEntrenamiento = nuevaRutina.LesionesPostEntrenamiento;
            SeguroAplicado = nuevaRutina.SeguroAplicado;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            var vencimiento = FechaVencimiento.HasValue ? $" - Vence: {FechaVencimiento.Value.ToShortDateString()}" : "";
            var lesiones = !string.IsNullOrEmpty(LesionesPostEntrenamiento) ? $" - Lesiones: {LesionesPostEntrenamiento}" : "";
            var seguro = SeguroAplicado != null ? $" - Seguro: {SeguroAplicado.NombreSeguro}" : "";

            return $"{Tipo} - {Duracion} min - {Intensidad} - {GrupoMuscular} - {NombreAtleta} - {FechaRealizacion.ToShortDateString()}{vencimiento}{lesiones}{seguro}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Rutina otraRutina)
            {
                return Id == otraRutina.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}
