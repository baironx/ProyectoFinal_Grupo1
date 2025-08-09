using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Servicios
{
    /// <summary>
    /// Servicio para análisis y estadísticas de entrenamiento.
    /// Implementa el principio Single Responsibility (SRP).
    /// </summary>
    public class ServicioEntrenamiento : IServicioEntrenamiento
    {
        #region Campos Privados

        private readonly IRepositorioRutinas<Rutina> _repositorioRutinas;
        private readonly IRepositorioSeguros<SeguroMedico> _repositorioSeguros;

        // Delegates para cálculos especializados
        public delegate double CalculadorEstadistica(IEnumerable<Rutina> rutinas);
        public delegate bool FiltroLesion(Rutina rutina, DateTime fechaLimite);

        private readonly Dictionary<string, CalculadorEstadistica> _calculadores;
        private readonly FiltroLesion _filtroLesiones;

        #endregion

        #region Constructor

        public ServicioEntrenamiento(IRepositorioRutinas<Rutina> repositorioRutinas,
                                   IRepositorioSeguros<SeguroMedico> repositorioSeguros)
        {
            _repositorioRutinas = repositorioRutinas ?? throw new ArgumentNullException(nameof(repositorioRutinas));
            _repositorioSeguros = repositorioSeguros ?? throw new ArgumentNullException(nameof(repositorioSeguros));

            // Inicializar calculadores con delegates
            _calculadores = new Dictionary<string, CalculadorEstadistica>
            {
                ["duracionPromedio"] = rutinas => rutinas.Any() ? rutinas.Average(r => r.Duracion) : 0,
                ["totalCalorías"] = rutinas => rutinas.Sum(r => r.CalcularCaloriasQuemadas()),
                ["intensidadPromedio"] = rutinas => CalcularIntensidadPromedio(rutinas)
            };

            // Filtro para lesiones usando delegate
            _filtroLesiones = (rutina, fechaLimite) =>
                !string.IsNullOrEmpty(rutina.LesionesPostEntrenamiento) &&
                rutina.FechaRealizacion >= fechaLimite;
        }

        #endregion

        #region Implementación de IServicioEntrenamiento

        /// <summary>
        /// Genera estadísticas completas de entrenamiento para un atleta.
        /// </summary>
        public EstadisticasAtleta GenerarEstadisticas(string nombreAtleta)
        {
            if (string.IsNullOrWhiteSpace(nombreAtleta))
                throw new ArgumentException("El nombre del atleta es requerido", nameof(nombreAtleta));

            var rutinas = _repositorioRutinas.ObtenerPorAtleta(nombreAtleta).ToList();
            var seguros = _repositorioSeguros.BuscarPorAtleta(nombreAtleta).ToList();

            var estadisticas = new EstadisticasAtleta(nombreAtleta);

            // Estadísticas básicas usando LINQ y delegates
            var totalRutinas = rutinas.Count;
            var duracionPromedio = _calculadores["duracionPromedio"](rutinas);
            var rutinasFuerza = rutinas.Count(r => r.Tipo.Equals("Fuerza", StringComparison.OrdinalIgnoreCase));
            var rutinasCardio = rutinas.Count(r => r.Tipo.Equals("Cardio", StringComparison.OrdinalIgnoreCase));

            estadisticas.ConfigurarEstadisticasBasicas(totalRutinas, duracionPromedio, rutinasFuerza, rutinasCardio);

            // Estadísticas de lesiones
            var totalLesiones = rutinas.Count(r => !string.IsNullOrEmpty(r.LesionesPostEntrenamiento));
            var lesionesUltimoMes = ObtenerLesionesUltimoMes(nombreAtleta).Count();
            var lesionesPorTipo = rutinas
                .Where(r => !string.IsNullOrEmpty(r.LesionesPostEntrenamiento))
                .GroupBy(r => r.LesionesPostEntrenamiento)
                .ToDictionary(g => g.Key, g => g.Count());

            estadisticas.ConfigurarEstadisticasLesiones(totalLesiones, lesionesUltimoMes, lesionesPorTipo);

            // Estadísticas de seguros
            var montoTotalSeguros = seguros.Sum(s => s.CalcularMontoTotal());
            estadisticas.ConfigurarEstadisticasSeguros(montoTotalSeguros);

            // Estadísticas del último mes
            var fechaLimite = DateTime.Today.AddMonths(-1);
            var rutinasUltimoMes = rutinas.Count(r => r.FechaRealizacion >= fechaLimite);
            estadisticas.ConfigurarEstadisticasUltimoMes(rutinasUltimoMes);

            // Estadísticas por intensidad
            var rutinasPorIntensidad = rutinas
                .GroupBy(r => r.Intensidad)
                .ToDictionary(g => g.Key, g => g.Count());
            estadisticas.ConfigurarRutinasPorIntensidad(rutinasPorIntensidad);

            return estadisticas;
        }

        /// <summary>
        /// Obtiene lesiones del último mes para un atleta.
        /// </summary>
        public IEnumerable<Rutina> ObtenerLesionesUltimoMes(string nombreAtleta)
        {
            if (string.IsNullOrWhiteSpace(nombreAtleta))
                return Enumerable.Empty<Rutina>();

            var fechaLimite = DateTime.Today.AddMonths(-1);
            var rutinas = _repositorioRutinas.ObtenerPorAtleta(nombreAtleta);

            // Usar delegate para filtrar lesiones
            return rutinas.Where(r => _filtroLesiones(r, fechaLimite));
        }

        /// <summary>
        /// Obtiene las 3 lesiones más frecuentes del último trimestre.
        /// </summary>
        public IEnumerable<IGrouping<string, Rutina>> ObtenerTop3LesionesTrimestre(string nombreAtleta)
        {
            if (string.IsNullOrWhiteSpace(nombreAtleta))
                return Enumerable.Empty<IGrouping<string, Rutina>>();

            var fechaLimite = DateTime.Today.AddMonths(-3);
            var rutinas = _repositorioRutinas.ObtenerPorAtleta(nombreAtleta);

            // Usar LINQ para obtener top 3 lesiones más frecuentes
            return rutinas
                .Where(r => r.FechaRealizacion >= fechaLimite && !string.IsNullOrEmpty(r.LesionesPostEntrenamiento))
                .GroupBy(r => r.LesionesPostEntrenamiento)
                .OrderByDescending(g => g.Count())
                .Take(3);
        }

        /// <summary>
        /// Genera análisis de seguros médicos para un atleta.
        /// </summary>
        public string GenerarAnalisisSeguros(string nombreAtleta)
        {
            if (string.IsNullOrWhiteSpace(nombreAtleta))
                return "Nombre de atleta requerido para análisis de seguros.";

            var seguros = _repositorioSeguros.BuscarPorAtleta(nombreAtleta).ToList();

            if (!seguros.Any())
                return $"No hay seguros médicos registrados para {nombreAtleta}.";

            var analisis = new System.Text.StringBuilder();
            analisis.AppendLine($"=== ANÁLISIS DE SEGUROS MÉDICOS - {nombreAtleta.ToUpper()} ===");
            analisis.AppendLine();

            // Estadísticas generales
            var totalSeguros = seguros.Count;
            var segurosActivos = seguros.Count(s => s.Estado == EstadoSeguro.Activo);
            var montoTotalCubierto = seguros.Sum(s => s.MontoCubierto);
            var montoTotalPaciente = seguros.Sum(s => s.MontoPaciente);
            var montoTotal = montoTotalCubierto + montoTotalPaciente;

            analisis.AppendLine("RESUMEN GENERAL:");
            analisis.AppendLine($"Total de seguros: {totalSeguros}");
            analisis.AppendLine($"Seguros activos: {segurosActivos}");
            analisis.AppendLine($"Monto total cubierto por seguros: ${montoTotalCubierto:F2}");
            analisis.AppendLine($"Monto total pagado por paciente: ${montoTotalPaciente:F2}");
            analisis.AppendLine($"Monto total de tratamientos: ${montoTotal:F2}");

            if (montoTotal > 0)
            {
                var porcentajeCobertura = (montoTotalCubierto / montoTotal) * 100;
                analisis.AppendLine($"Porcentaje de cobertura promedio: {porcentajeCobertura:F1}%");
            }

            analisis.AppendLine();

            // Análisis por tipo de lesión usando LINQ
            var lesionMasFrecuente = seguros
                .GroupBy(s => s.LesionTratada)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (lesionMasFrecuente != null)
            {
                analisis.AppendLine("ANÁLISIS DE LESIONES:");
                analisis.AppendLine($"Lesión más frecuente: {lesionMasFrecuente.Key} ({lesionMasFrecuente.Count()} ocurrencias)");

                var montoLesionMasFrecuente = lesionMasFrecuente.Sum(s => s.CalcularMontoTotal());
                analisis.AppendLine($"Costo total de lesión más frecuente: ${montoLesionMasFrecuente:F2}");
            }

            analisis.AppendLine();

            // Análisis por seguro médico
            var seguroMasUtilizado = seguros
                .GroupBy(s => s.NombreSeguro)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (seguroMasUtilizado != null)
            {
                analisis.AppendLine("ANÁLISIS DE SEGUROS:");
                analisis.AppendLine($"Seguro más utilizado: {seguroMasUtilizado.Key} ({seguroMasUtilizado.Count()} usos)");

                var coberturaPromedio = seguroMasUtilizado.Average(s => s.CalcularPorcentajeCobertura());
                analisis.AppendLine($"Cobertura promedio del seguro principal: {coberturaPromedio:F1}%");
            }

            analisis.AppendLine();

            // Tendencias temporales
            var segurosUltimoMes = seguros.Count(s => s.FechaAplicacion >= DateTime.Today.AddMonths(-1));
            var segurosUltimoTrimestre = seguros.Count(s => s.FechaAplicacion >= DateTime.Today.AddMonths(-3));

            analisis.AppendLine("TENDENCIAS TEMPORALES:");
            analisis.AppendLine($"Seguros aplicados último mes: {segurosUltimoMes}");
            analisis.AppendLine($"Seguros aplicados último trimestre: {segurosUltimoTrimestre}");

            // Recomendaciones
            analisis.AppendLine();
            analisis.AppendLine("RECOMENDACIONES:");

            if (segurosUltimoMes > 2)
            {
                analisis.AppendLine("Alta frecuencia de lesiones recientes. Revisar intensidad de entrenamientos.");
            }

            if (montoTotalPaciente > montoTotalCubierto)
            {
                analisis.AppendLine("Considerar mejorar cobertura del seguro médico.");
            }

            if (totalSeguros == 0)
            {
                analisis.AppendLine("Sin lesiones reportadas. Mantener rutina preventiva.");
            }

            return analisis.ToString();
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Calcula la intensidad promedio de un conjunto de rutinas.
        /// </summary>
        private double CalcularIntensidadPromedio(IEnumerable<Rutina> rutinas)
        {
            if (!rutinas.Any()) return 0;

            var sumaIntensidades = rutinas.Sum(r => r.Intensidad.ToLower() switch
            {
                "baja" => 1,
                "media" => 2,
                "alta" => 3,
                _ => 0
            });

            return (double)sumaIntensidades / rutinas.Count();
        }

        #endregion

        #region Métodos Adicionales con LINQ

        /// <summary>
        /// Obtiene rutinas que requieren seguimiento médico.
        /// </summary>
        public IEnumerable<Rutina> ObtenerRutinasConSeguimiento(string nombreAtleta)
        {
            var rutinas = _repositorioRutinas.ObtenerPorAtleta(nombreAtleta);

            return rutinas.Where(r =>
                !string.IsNullOrEmpty(r.LesionesPostEntrenamiento) ||
                r.SeguroAplicado != null ||
                r.Intensidad.Equals("Alta", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Calcula el riesgo de lesión basado en patrones de entrenamiento.
        /// </summary>
        public double CalcularRiesgoLesion(string nombreAtleta)
        {
            var rutinas = _repositorioRutinas.ObtenerPorAtleta(nombreAtleta).ToList();

            if (!rutinas.Any()) return 0;

            var factorRiesgo = 0.0;

            // Factor por frecuencia de lesiones
            var porcentajeLesiones = (double)rutinas.Count(r => !string.IsNullOrEmpty(r.LesionesPostEntrenamiento)) / rutinas.Count * 100;
            factorRiesgo += porcentajeLesiones * 0.4;

            // Factor por intensidad promedio
            var intensidadPromedio = _calculadores["intensidadPromedio"](rutinas);
            factorRiesgo += intensidadPromedio * 15; // Máximo 45 puntos

            // Factor por frecuencia de entrenamientos de alta intensidad
            var porcentajeAltaIntensidad = (double)rutinas.Count(r => r.Intensidad.Equals("Alta", StringComparison.OrdinalIgnoreCase)) / rutinas.Count * 100;
            factorRiesgo += porcentajeAltaIntensidad * 0.3;

            // Factor por rutinas recientes sin descanso
            var ultimaSemana = DateTime.Today.AddDays(-7);
            var rutinasRecientes = rutinas.Count(r => r.FechaRealizacion >= ultimaSemana);
            if (rutinasRecientes > 5) factorRiesgo += 10; // Sobreentrenaimiento

            return Math.Min(factorRiesgo, 100); // Máximo 100%
        }

        /// <summary>
        /// Genera recomendaciones personalizadas basadas en el análisis.
        /// </summary>
        public List<string> GenerarRecomendaciones(string nombreAtleta)
        {
            var recomendaciones = new List<string>();
            var rutinas = _repositorioRutinas.ObtenerPorAtleta(nombreAtleta).ToList();
            var riesgoLesion = CalcularRiesgoLesion(nombreAtleta);

            // Recomendaciones basadas en riesgo de lesión
            if (riesgoLesion > 70)
            {
                recomendaciones.Add("ALTO RIESGO: Reducir intensidad y aumentar días de descanso");
                recomendaciones.Add("Considerar evaluación médica preventiva");
            }
            else if (riesgoLesion > 40)
            {
                recomendaciones.Add("RIESGO MODERADO: Incluir más ejercicios de recuperación");
                recomendaciones.Add("Implementar técnicas de relajación y estiramiento");
            }

            // Recomendaciones basadas en patrones de entrenamiento
            var ultimoMes = DateTime.Today.AddMonths(-1);
            var rutinasUltimoMes = rutinas.Where(r => r.FechaRealizacion >= ultimoMes).ToList();

            if (rutinasUltimoMes.Count < 8)
            {
                recomendaciones.Add("Aumentar frecuencia de entrenamiento (mínimo 2 por semana)");
            }
            else if (rutinasUltimoMes.Count > 20)
            {
                recomendaciones.Add("Incluir más días de descanso para evitar sobreentrenamiento");
            }

            // Recomendaciones por tipo de rutina
            var porcentajeCardio = (double)rutinas.Count(r => r.Tipo == "Cardio") / rutinas.Count * 100;
            var porcentajeFuerza = (double)rutinas.Count(r => r.Tipo == "Fuerza") / rutinas.Count * 100;

            if (porcentajeCardio < 30)
            {
                recomendaciones.Add("Incluir más ejercicios cardiovasculares (mínimo 30% del total)");
            }
            if (porcentajeFuerza < 30)
            {
                recomendaciones.Add("Incluir más ejercicios de fuerza (mínimo 30% del total)");
            }

            // Recomendaciones basadas en lesiones
            var lesionesRecurrentes = rutinas
                .Where(r => !string.IsNullOrEmpty(r.LesionesPostEntrenamiento))
                .GroupBy(r => r.LesionesPostEntrenamiento)
                .Where(g => g.Count() > 1)
                .ToList();

            if (lesionesRecurrentes.Any())
            {
                recomendaciones.Add("Lesiones recurrentes detectadas. Consultar especialista en medicina deportiva");
                foreach (var lesion in lesionesRecurrentes)
                {
                    recomendaciones.Add($"   • {lesion.Key}: {lesion.Count()} ocurrencias");
                }
            }

            if (!recomendaciones.Any())
            {
                recomendaciones.Add("Perfil de entrenamiento saludable. Continuar con la rutina actual");
                recomendaciones.Add("Considerar progresión gradual en intensidad");
            }

            return recomendaciones;
        }

        #endregion
    }
}