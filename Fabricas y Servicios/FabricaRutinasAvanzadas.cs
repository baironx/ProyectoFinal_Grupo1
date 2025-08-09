using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;

namespace AppEntrenamientoPersonal.Fabricas
{
    /// <summary>
    /// Fábrica para crear rutinas avanzadas y especializadas.
    /// </summary>
    public static class FabricaRutinasAvanzadas
    {
        /// <summary>
        /// Delegate para estrategias de progresión.
        /// </summary>
        public delegate Rutina EstrategiaProgresion(Rutina rutinaAnterior, int semanas);

        /// <summary>
        /// Crea un programa de entrenamiento progresivo.
        /// </summary>
        public static List<Rutina> CrearProgramaProgresivo(Atleta atleta, string objetivo, int duracionSemanas)
        {
            var rutinas = new List<Rutina>();
            var fechaInicio = DateTime.Today;

            // Estrategia de progresión usando delegate
            EstrategiaProgresion estrategia = objetivo.ToLower() switch
            {
                "fuerza" => (rutinaAnterior, semanas) => ProgresarFuerza(rutinaAnterior, semanas),
                "resistencia" => (rutinaAnterior, semanas) => ProgresarResistencia(rutinaAnterior, semanas),
                "perdida_peso" => (rutinaAnterior, semanas) => ProgresarPerdidaPeso(rutinaAnterior, semanas),
                _ => (rutinaAnterior, semanas) => ProgresarGeneral(rutinaAnterior, semanas)
            };

            // Crear rutina base
            var rutinaBase = CrearRutinaBase(atleta, objetivo);
            rutinas.Add(rutinaBase);

            // Generar progresión
            var rutinaActual = rutinaBase;
            for (int semana = 1; semana < duracionSemanas; semana++)
            {
                rutinaActual = estrategia(rutinaActual, semana);
                rutinas.Add(rutinaActual);
            }

            return rutinas;
        }

        /// <summary>
        /// Crea un circuito de entrenamiento funcional.
        /// </summary>
        public static List<Rutina> CrearCircuitoFuncional(Atleta atleta, int numeroEstaciones = 6)
        {
            var circuito = new List<Rutina>();
            var fechaHoy = DateTime.Today;
            var gruposMusculares = new[] { "Pecho", "Espalda", "Piernas", "Brazos", "Hombros", "Abdomen" };
            var tiposCardio = new[] { "Burpees", "Mountain Climbers", "Jumping Jacks", "High Knees" };

            for (int i = 0; i < numeroEstaciones; i++)
            {
                if (i % 2 == 0) // Estaciones de fuerza
                {
                    var grupoMuscular = gruposMusculares[i % gruposMusculares.Length];
                    circuito.Add(FabricaRutinas.CrearRutinaFuerza(
                        duracion: 3, // 3 minutos por estación
                        intensidad: "Alta",
                        grupoMuscular: grupoMuscular,
                        nombreAtleta: atleta.Nombre,
                        fechaRealizacion: fechaHoy,
                        series: 1,
                        repeticiones: 45, // 45 segundos de trabajo
                        peso: 0
                    ));
                }
                else // Estaciones de cardio
                {
                    var tipoCardio = tiposCardio[i % tiposCardio.Length];
                    circuito.Add(FabricaRutinas.CrearRutinaCardio(
                        duracion: 2, // 2 minutos por estación
                        intensidad: "Alta",
                        grupoMuscular: "Cardio",
                        nombreAtleta: atleta.Nombre,
                        fechaRealizacion: fechaHoy,
                        tipoCardio: tipoCardio,
                        distancia: 0,
                        frecuenciaCardiaca: 160
                    ));
                }
            }

            return circuito;
        }

        /// <summary>
        /// Crea rutinas especializadas por deporte.
        /// </summary>
        public static List<Rutina> CrearRutinasDeporteEspecifico(Atleta atleta, string deporte)
        {
            var rutinas = new List<Rutina>();
            var fechaHoy = DateTime.Today;

            switch (deporte.ToLower())
            {
                case "futbol":
                    rutinas.AddRange(CrearRutinasFutbol(atleta, fechaHoy));
                    break;
                case "natacion":
                    rutinas.AddRange(CrearRutinasNatacion(atleta, fechaHoy));
                    break;
                case "ciclismo":
                    rutinas.AddRange(CrearRutinasCiclismo(atleta, fechaHoy));
                    break;
                case "atletismo":
                    rutinas.AddRange(CrearRutinasAtletismo(atleta, fechaHoy));
                    break;
                default:
                    rutinas.AddRange(CrearRutinasGenerales(atleta, fechaHoy));
                    break;
            }

            return rutinas;
        }

        #region Métodos Privados de Progresión

        private static Rutina ProgresarFuerza(Rutina rutinaAnterior, int semanas)
        {
            if (rutinaAnterior is RutinaFuerza fuerza)
            {
                var nuevoPeso = fuerza.PesoUtilizado + (semanas * 2.5); // Incremento progresivo
                var nuevasSeries = Math.Min(fuerza.Series + (semanas / 2), 5); // Máximo 5 series

                return FabricaRutinas.CrearRutinaFuerza(
                    fuerza.Duracion + (semanas * 2),
                    fuerza.Intensidad,
                    fuerza.GrupoMuscular,
                    fuerza.NombreAtleta,
                    DateTime.Today.AddDays(semanas * 7),
                    nuevasSeries,
                    fuerza.Repeticiones,
                    nuevoPeso
                );
            }
            return rutinaAnterior;
        }

        private static Rutina ProgresarResistencia(Rutina rutinaAnterior, int semanas)
        {
            if (rutinaAnterior is RutinaCardio cardio)
            {
                var nuevaDuracion = cardio.Duracion + (semanas * 3);
                var nuevaDistancia = cardio.DistanciaRecorrida + (semanas * 0.5);

                return FabricaRutinas.CrearRutinaCardio(
                    nuevaDuracion,
                    cardio.Intensidad,
                    cardio.GrupoMuscular,
                    cardio.NombreAtleta,
                    DateTime.Today.AddDays(semanas * 7),
                    cardio.TipoCardio,
                    nuevaDistancia,
                    cardio.FrecuenciaCardiacaPromedio
                );
            }
            return rutinaAnterior;
        }

        private static Rutina ProgresarPerdidaPeso(Rutina rutinaAnterior, int semanas)
        {
            // Alternar entre fuerza y cardio, con énfasis en cardio
            if (rutinaAnterior is RutinaCardio)
            {
                return ProgresarResistencia(rutinaAnterior, semanas);
            }
            else
            {
                // Crear rutina de cardio para pérdida de peso
                return FabricaRutinas.CrearRutinaCardio(
                    30 + (semanas * 3),
                    "Media",
                    "Cardio",
                    rutinaAnterior.NombreAtleta,
                    DateTime.Today.AddDays(semanas * 7),
                    "HIIT",
                    3.0 + (semanas * 0.3),
                    150 + (semanas * 2)
                );
            }
        }

        private static Rutina ProgresarGeneral(Rutina rutinaAnterior, int semanas)
        {
            // Progresión equilibrada
            var nuevaDuracion = rutinaAnterior.Duracion + (semanas * 2);
            var nuevaIntensidad = semanas > 4 ? "Media" : rutinaAnterior.Intensidad;
            if (semanas > 8) nuevaIntensidad = "Alta";

            if (rutinaAnterior is RutinaFuerza)
            {
                return ProgresarFuerza(rutinaAnterior, semanas);
            }
            else
            {
                return ProgresarResistencia(rutinaAnterior, semanas);
            }
        }

        private static Rutina CrearRutinaBase(Atleta atleta, string objetivo)
        {
            return objetivo.ToLower() switch
            {
                "fuerza" => FabricaRutinas.CrearRutinaFuerza(30, "Baja", "Pecho", atleta.Nombre, DateTime.Today),
                "resistencia" => FabricaRutinas.CrearRutinaCardio(20, "Baja", "Cardio", atleta.Nombre, DateTime.Today),
                "perdida_peso" => FabricaRutinas.CrearRutinaCardio(25, "Media", "Cardio", atleta.Nombre, DateTime.Today, "HIIT"),
                _ => FabricaRutinas.CrearRutinaFuerza(25, "Baja", "General", atleta.Nombre, DateTime.Today)
            };
        }

        #endregion

        #region Rutinas Específicas por Deporte

        private static List<Rutina> CrearRutinasFutbol(Atleta atleta, DateTime fecha)
        {
            return new List<Rutina>
            {
                FabricaRutinas.CrearRutinaFuerza(45, "Media", "Piernas", atleta.Nombre, fecha, 3, 12, 30),
                FabricaRutinas.CrearRutinaCardio(30, "Alta", "Cardio", atleta.Nombre, fecha, "Sprints", 5.0, 170),
                FabricaRutinas.CrearRutinaFuerza(35, "Media", "Abdomen", atleta.Nombre, fecha.AddDays(1), 4, 20, 0)
            };
        }

        private static List<Rutina> CrearRutinasNatacion(Atleta atleta, DateTime fecha)
        {
            return new List<Rutina>
            {
                FabricaRutinas.CrearRutinaFuerza(40, "Media", "Hombros", atleta.Nombre, fecha, 3, 15, 15),
                FabricaRutinas.CrearRutinaCardio(60, "Media", "Cardio", atleta.Nombre, fecha, "Natación", 2.0, 140),
                FabricaRutinas.CrearRutinaFuerza(30, "Baja", "Espalda", atleta.Nombre, fecha.AddDays(1), 3, 12, 20)
            };
        }

        private static List<Rutina> CrearRutinasCiclismo(Atleta atleta, DateTime fecha)
        {
            return new List<Rutina>
            {
                FabricaRutinas.CrearRutinaCardio(90, "Media", "Cardio", atleta.Nombre, fecha, "Bicicleta", 25.0, 130),
                FabricaRutinas.CrearRutinaFuerza(35, "Media", "Piernas", atleta.Nombre, fecha.AddDays(1), 4, 10, 40),
                FabricaRutinas.CrearRutinaFuerza(25, "Baja", "Abdomen", atleta.Nombre, fecha.AddDays(2), 3, 25, 0)
            };
        }

        private static List<Rutina> CrearRutinasAtletismo(Atleta atleta, DateTime fecha)
        {
            return new List<Rutina>
            {
                FabricaRutinas.CrearRutinaCardio(45, "Alta", "Cardio", atleta.Nombre, fecha, "Correr", 10.0, 160),
                FabricaRutinas.CrearRutinaFuerza(50, "Alta", "Piernas", atleta.Nombre, fecha.AddDays(1), 4, 8, 50),
                FabricaRutinas.CrearRutinaFuerza(30, "Media", "Brazos", atleta.Nombre, fecha.AddDays(2), 3, 12, 25)
            };
        }

        private static List<Rutina> CrearRutinasGenerales(Atleta atleta, DateTime fecha)
        {
            return new List<Rutina>
            {
                FabricaRutinas.CrearRutinaFuerza(40, "Media", "Pecho", atleta.Nombre, fecha, 3, 12, 25),
                FabricaRutinas.CrearRutinaCardio(30, "Media", "Cardio", atleta.Nombre, fecha.AddDays(1), "Trote", 5.0, 140),
                FabricaRutinas.CrearRutinaFuerza(35, "Media", "Espalda", atleta.Nombre, fecha.AddDays(2), 3, 10, 30)
            };
        }

        #endregion
    }
}