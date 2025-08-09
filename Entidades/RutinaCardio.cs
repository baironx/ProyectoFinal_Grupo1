using System;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Entidades
{
    /// <summary>
    /// Rutina específica para entrenamiento cardiovascular.
    /// Hereda de Rutina e implementa comportamientos específicos.
    /// </summary>
    public sealed class RutinaCardio : Rutina
    {
        #region Propiedades Específicas

        public override string Tipo => "Cardio";
        public double DistanciaRecorrida { get; private set; }
        public int FrecuenciaCardiacaPromedio { get; private set; }
        public string TipoCardio { get; private set; }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor principal para rutina de cardio.
        /// </summary>
        public RutinaCardio(int duracion, string intensidad, string grupoMuscular, string nombreAtleta,
                           DateTime fechaRealizacion, DateTime? fechaVencimiento, string lesionesPostEntrenamiento,
                           SeguroMedico seguroAplicado = null!, double distanciaRecorrida = 0,
                           int frecuenciaCardiacaPromedio = 0, string tipoCardio = "General")
            : base(duracion, intensidad, grupoMuscular, nombreAtleta, fechaRealizacion, fechaVencimiento, lesionesPostEntrenamiento, seguroAplicado)
        {
            DistanciaRecorrida = distanciaRecorrida;
            FrecuenciaCardiacaPromedio = frecuenciaCardiacaPromedio;
            TipoCardio = tipoCardio ?? "General";
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Implementación específica para describir rutina de cardio.
        /// </summary>
        public override string Describir()
        {
            var descripcionBase = $"[Cardio] {Duracion} min - {Intensidad} - {TipoCardio} - {FechaRealizacion.ToShortDateString()}";

            if (DistanciaRecorrida > 0)
                descripcionBase += $" - {DistanciaRecorrida:F2}km";

            if (FrecuenciaCardiacaPromedio > 0)
                descripcionBase += $" - {FrecuenciaCardiacaPromedio}bpm";

            return descripcionBase;
        }

        /// <summary>
        /// Obtiene características específicas de la rutina de cardio.
        /// </summary>
        public override string ObtenerCaracteristicasEspecificas()
        {
            var velocidadPromedio = CalcularVelocidadPromedio();
            return $"Tipo: {TipoCardio}, Distancia: {DistanciaRecorrida}km, FC Promedio: {FrecuenciaCardiacaPromedio}bpm, Velocidad: {velocidadPromedio:F2}km/h";
        }

        /// <summary>
        /// Cálculo específico de calorías para rutinas de cardio.
        /// </summary>
        public override double CalcularCaloriasQuemadas()
        {
            var caloriesBase = base.CalcularCaloriasQuemadas();

            // Factor adicional por frecuencia cardíaca y distancia
            var factorFC = FrecuenciaCardiacaPromedio > 140 ? 1.3 : 1.0;
            var factorDistancia = DistanciaRecorrida > 0 ? 1 + (DistanciaRecorrida / 10) : 1.0;

            return caloriesBase * factorFC * factorDistancia;
        }

        /// <summary>
        /// Las rutinas de cardio pueden necesitar equipo según el tipo.
        /// </summary>
        public override bool NecesitaEquipoEspecial()
        {
            return TipoCardio.ToLower() switch
            {
                "bicicleta" or "spinning" => true,
                "cinta" or "elíptica" => true,
                "natación" => true,
                _ => false
            };
        }

        #endregion

        #region Métodos Específicos

        /// <summary>
        /// Calcula la velocidad promedio durante el ejercicio.
        /// </summary>
        public double CalcularVelocidadPromedio()
        {
            if (DistanciaRecorrida <= 0 || Duracion <= 0) return 0;
            return (DistanciaRecorrida / Duracion) * 60; // km/h
        }

        /// <summary>
        /// Determina la zona de frecuencia cardíaca.
        /// </summary>
        public string DeterminarZonaFC(int edadAtleta = 30)
        {
            if (FrecuenciaCardiacaPromedio <= 0) return "No especificada";

            var fcMaxima = 220 - edadAtleta;
            var porcentaje = (double)FrecuenciaCardiacaPromedio / fcMaxima * 100;

            return porcentaje switch
            {
                >= 90 => "Zona Anaeróbica (90-100%)",
                >= 80 => "Zona Umbral (80-90%)",
                >= 70 => "Zona Aeróbica (70-80%)",
                >= 60 => "Zona Quema Grasa (60-70%)",
                >= 50 => "Zona Recuperación (50-60%)",
                _ => "Zona Muy Ligera (<50%)"
            };
        }

        /// <summary>
        /// Calcula el gasto metabólico equivalente (MET).
        /// </summary>
        public double CalcularMET()
        {
            return TipoCardio.ToLower() switch
            {
                "caminata" => Intensidad.ToLower() switch
                {
                    "baja" => 3.0,
                    "media" => 4.0,
                    "alta" => 5.0,
                    _ => 3.5
                },
                "trote" or "correr" => Intensidad.ToLower() switch
                {
                    "baja" => 6.0,
                    "media" => 8.0,
                    "alta" => 12.0,
                    _ => 8.0
                },
                "bicicleta" => Intensidad.ToLower() switch
                {
                    "baja" => 4.0,
                    "media" => 6.0,
                    "alta" => 10.0,
                    _ => 6.0
                },
                "natación" => Intensidad.ToLower() switch
                {
                    "baja" => 6.0,
                    "media" => 8.0,
                    "alta" => 11.0,
                    _ => 8.0
                },
                _ => 5.0
            };
        }

        #endregion
    }
}
