using System;
using System.Collections.Generic;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Entidades
{
    /// <summary>
    /// Representa las estadísticas compiladas de un atleta.
    /// Utilizada para generar reportes y análisis.
    /// </summary>
    public sealed class EstadisticasAtleta : IDescribible
    {
        #region Propiedades

        public string NombreAtleta { get; private set; }
        public int TotalRutinas { get; private set; }
        public double DuracionPromedioRutinas { get; private set; }
        public int TotalRutinasFuerza { get; private set; }
        public int TotalRutinasCardio { get; private set; }
        public int TotalLesiones { get; private set; }
        public decimal MontoTotalSeguros { get; private set; }
        public int RutinasUltimoMes { get; private set; }
        public int LesionesUltimoMes { get; private set; }
        public DateTime FechaGeneracion { get; private set; }
        public Dictionary<string, int> LesionesPorTipo { get; private set; }
        public Dictionary<string, int> RutinasPorIntensidad { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor para estadísticas de atleta.
        /// </summary>
        public EstadisticasAtleta(string nombreAtleta)
        {
            NombreAtleta = nombreAtleta ?? throw new ArgumentNullException(nameof(nombreAtleta));
            FechaGeneracion = DateTime.Now;
            LesionesPorTipo = new Dictionary<string, int>();
            RutinasPorIntensidad = new Dictionary<string, int>();
        }

        #endregion

        #region Métodos de Configuración

        /// <summary>
        /// Configura las estadísticas básicas.
        /// </summary>
        public EstadisticasAtleta ConfigurarEstadisticasBasicas(int totalRutinas, double duracionPromedio,
                                                               int rutinasFuerza, int rutinasCardio)
        {
            TotalRutinas = totalRutinas;
            DuracionPromedioRutinas = duracionPromedio;
            TotalRutinasFuerza = rutinasFuerza;
            TotalRutinasCardio = rutinasCardio;
            return this;
        }

        /// <summary>
        /// Configura las estadísticas de lesiones.
        /// </summary>
        public EstadisticasAtleta ConfigurarEstadisticasLesiones(int totalLesiones, int lesionesUltimoMes,
                                                               Dictionary<string, int> lesionesPorTipo)
        {
            TotalLesiones = totalLesiones;
            LesionesUltimoMes = lesionesUltimoMes;
            LesionesPorTipo = lesionesPorTipo ?? new Dictionary<string, int>();
            return this;
        }

        /// <summary>
        /// Configura las estadísticas de seguros.
        /// </summary>
        public EstadisticasAtleta ConfigurarEstadisticasSeguros(decimal montoTotalSeguros)
        {
            MontoTotalSeguros = montoTotalSeguros;
            return this;
        }

        /// <summary>
        /// Configura estadísticas del último mes.
        /// </summary>
        public EstadisticasAtleta ConfigurarEstadisticasUltimoMes(int rutinasUltimoMes)
        {
            RutinasUltimoMes = rutinasUltimoMes;
            return this;
        }

        /// <summary>
        /// Configura estadísticas por intensidad.
        /// </summary>
        public EstadisticasAtleta ConfigurarRutinasPorIntensidad(Dictionary<string, int> rutinasPorIntensidad)
        {
            RutinasPorIntensidad = rutinasPorIntensidad ?? new Dictionary<string, int>();
            return this;
        }

        #endregion

        #region Métodos de Análisis

        /// <summary>
        /// Calcula el porcentaje de rutinas de fuerza.
        /// </summary>
        public double CalcularPorcentajeFuerza()
        {
            return TotalRutinas > 0 ? (double)TotalRutinasFuerza / TotalRutinas * 100 : 0;
        }

        /// <summary>
        /// Calcula el porcentaje de rutinas de cardio.
        /// </summary>
        public double CalcularPorcentajeCardio()
        {
            return TotalRutinas > 0 ? (double)TotalRutinasCardio / TotalRutinas * 100 : 0;
        }

        /// <summary>
        /// Calcula la tasa de lesiones por rutina.
        /// </summary>
        public double CalcularTasaLesiones()
        {
            return TotalRutinas > 0 ? (double)TotalLesiones / TotalRutinas * 100 : 0;
        }

        /// <summary>
        /// Obtiene la intensidad más utilizada.
        /// </summary>
        /// <returns>Intensidad más frecuente.</returns>
        public string ObtenerIntensidadMasFrecuente()
        {
            var maxValue = 0;
            var intensidadMasFrecuente = "No especificada";

            foreach (var kvp in RutinasPorIntensidad)
            {
                if (kvp.Value > maxValue)
                {
                    maxValue = kvp.Value;
                    intensidadMasFrecuente = kvp.Key;
                }
            }

            return intensidadMasFrecuente;
        }

        /// <summary>
        /// Obtiene la lesión más frecuente.
        /// </summary>
        /// <returns>Lesión más frecuente.</returns>
        public string ObtenerLesionMasFrecuente()
        {
            var maxValue = 0;
            var lesionMasFrecuente = "Ninguna";

            foreach (var kvp in LesionesPorTipo)
            {
                if (kvp.Value > maxValue)
                {
                    maxValue = kvp.Value;
                    lesionMasFrecuente = kvp.Key;
                }
            }

            return lesionMasFrecuente;
        }

        #endregion

        #region Implementación de Interfaces

        /// <summary>
        /// Implementación de IDescribible.
        /// </summary>
        public string Describir()
        {
            return ToString();
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            var estadisticas = new System.Text.StringBuilder();

            estadisticas.AppendLine($"=== Estadisticas de {NombreAtleta.ToUpper()} ===");
            estadisticas.AppendLine($"Generado: {FechaGeneracion:dd/MM/yyyy HH:mm}");
            estadisticas.AppendLine();

            estadisticas.AppendLine("RUTINAS:");
            estadisticas.AppendLine($"  • Total de rutinas: {TotalRutinas}");
            estadisticas.AppendLine($"  • Duración promedio: {DuracionPromedioRutinas:F1} minutos");
            estadisticas.AppendLine($"  • Rutinas de fuerza: {TotalRutinasFuerza} ({CalcularPorcentajeFuerza():F1}%)");
            estadisticas.AppendLine($"  • Rutinas de cardio: {TotalRutinasCardio} ({CalcularPorcentajeCardio():F1}%)");
            estadisticas.AppendLine($"  • Rutinas último mes: {RutinasUltimoMes}");
            estadisticas.AppendLine();

            estadisticas.AppendLine("LESIONES:");
            estadisticas.AppendLine($"  • Total de lesiones: {TotalLesiones}");
            estadisticas.AppendLine($"  • Lesiones último mes: {LesionesUltimoMes}");
            estadisticas.AppendLine($"  • Tasa de lesiones: {CalcularTasaLesiones():F2}% por rutina");
            estadisticas.AppendLine($"  • Lesión más frecuente: {ObtenerLesionMasFrecuente()}");
            estadisticas.AppendLine();

            estadisticas.AppendLine("SEGUROS:");
            estadisticas.AppendLine($"  • Monto total cubierto: ₡{MontoTotalSeguros:F2}");
            estadisticas.AppendLine();

            estadisticas.AppendLine("ANÁLISIS:");
            estadisticas.AppendLine($"  • Intensidad más utilizada: {ObtenerIntensidadMasFrecuente()}");

            return estadisticas.ToString();
        }

        #endregion
    }
}
