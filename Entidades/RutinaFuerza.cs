using System;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Entidades
{
    /// <summary>
    /// Rutina específica para entrenamiento de fuerza.
    /// Hereda de Rutina e implementa comportamientos específicos.
    /// </summary>
    public sealed class RutinaFuerza : Rutina
    {
        #region Propiedades Específicas

        public override string Tipo => "Fuerza";
        public int Series { get; private set; }
        public int Repeticiones { get; private set; }
        public double PesoUtilizado { get; private set; }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor principal para rutina de fuerza.
        /// </summary>
        public RutinaFuerza(int duracion, string intensidad, string grupoMuscular, string nombreAtleta,
                           DateTime fechaRealizacion, DateTime? fechaVencimiento, string lesionesPostEntrenamiento,
                           SeguroMedico seguroAplicado = null!, int series = 3, int repeticiones = 10, double pesoUtilizado = 0)
            : base(duracion, intensidad, grupoMuscular, nombreAtleta, fechaRealizacion, fechaVencimiento, lesionesPostEntrenamiento, seguroAplicado)
        {
            Series = series;
            Repeticiones = repeticiones;
            PesoUtilizado = pesoUtilizado;
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Implementación específica para describir rutina de fuerza.
        /// </summary>
        public override string Describir()
        {
            var descripcionBase = $"[Fuerza] {Duracion} min - {Intensidad} - {GrupoMuscular} - {FechaRealizacion.ToShortDateString()}";

            if (Series > 0 && Repeticiones > 0)
                descripcionBase += $" - {Series}x{Repeticiones}";

            if (PesoUtilizado > 0)
                descripcionBase += $" - {PesoUtilizado}kg";

            return descripcionBase;
        }

        /// <summary>
        /// Obtiene características específicas de la rutina de fuerza.
        /// </summary>
        public override string ObtenerCaracteristicasEspecificas()
        {
            return $"Series: {Series}, Repeticiones: {Repeticiones}, Peso: {PesoUtilizado}kg, Volumen total: {CalcularVolumenTotal()}kg";
        }

        /// <summary>
        /// Cálculo específico de calorías para rutinas de fuerza.
        /// </summary>
        public override double CalcularCaloriasQuemadas()
        {
            var caloriesBase = base.CalcularCaloriasQuemadas();

            // Factor adicional por peso utilizado y volumen
            var factorPeso = PesoUtilizado > 0 ? 1 + (PesoUtilizado / 100) : 1;
            var factorVolumen = Series * Repeticiones > 30 ? 1.2 : 1.0;

            return caloriesBase * factorPeso * factorVolumen;
        }

        /// <summary>
        /// Las rutinas de fuerza típicamente necesitan equipo.
        /// </summary>
        public override bool NecesitaEquipoEspecial()
        {
            return PesoUtilizado > 0 || base.NecesitaEquipoEspecial();
        }

        #endregion

        #region Métodos Específicos

        /// <summary>
        /// Calcula el volumen total de entrenamiento (series x repeticiones x peso).
        /// </summary>
        public double CalcularVolumenTotal()
        {
            return Series * Repeticiones * PesoUtilizado;
        }

        /// <summary>
        /// Determina la intensidad relativa basada en el peso y repeticiones.
        /// </summary>
        public string CalcularIntensidadRelativa()
        {
            if (Repeticiones <= 5) return "Fuerza máxima";
            if (Repeticiones <= 8) return "Fuerza";
            if (Repeticiones <= 12) return "Hipertrofia";
            return "Resistencia muscular";
        }

        /// <summary>
        /// Sugiere el tiempo de descanso entre series.
        /// </summary>
        public int SugerirTiempoDescanso()
        {
            return Intensidad.ToLower() switch
            {
                "alta" => 180, // 3 minutos
                "media" => 120, // 2 minutos
                "baja" => 60,   // 1 minuto
                _ => 90
            };
        }

        #endregion
    }
}
