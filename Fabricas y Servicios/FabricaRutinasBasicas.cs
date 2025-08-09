using System;
using System.Collections.Generic;
using AppEntrenamientoPersonal.Entidades;

namespace AppEntrenamientoPersonal.Fabricas
{
    /// <summary>
    /// Fábrica para crear rutinas básicas predefinidas.
    /// </summary>
    public static class FabricaRutinasBasicas
    {
        /// <summary>
        /// Delegate para personalización de rutinas.
        /// </summary>
        public delegate Rutina PersonalizadorRutina(Rutina rutina, Atleta atleta);

        /// <summary>
        /// Crea una rutina básica de pecho para principiantes.
        /// </summary>
        public static RutinaFuerza CrearRutinaPechoPrincipiante(string nombreAtleta, DateTime fechaRealizacion)
        {
            return FabricaRutinas.CrearRutinaFuerza(
                duracion: 30,
                intensidad: "Baja",
                grupoMuscular: "Pecho",
                nombreAtleta: nombreAtleta,
                fechaRealizacion: fechaRealizacion,
                series: 3,
                repeticiones: 12,
                peso: 0
            );
        }

        /// <summary>
        /// Crea una rutina básica de cardio para principiantes.
        /// </summary>
        public static RutinaCardio CrearRutinaCardioPrincipiante(string nombreAtleta, DateTime fechaRealizacion)
        {
            return FabricaRutinas.CrearRutinaCardio(
                duracion: 20,
                intensidad: "Baja",
                grupoMuscular: "Cardio",
                nombreAtleta: nombreAtleta,
                fechaRealizacion: fechaRealizacion,
                tipoCardio: "Caminata",
                distancia: 2.0,
                frecuenciaCardiaca: 120
            );
        }

        /// <summary>
        /// Crea rutinas específicas basadas en el nivel del atleta.
        /// </summary>
        public static List<Rutina> CrearRutinasPorNivel(Atleta atleta, string tipoRutina, string grupoMuscular)
        {
            var rutinas = new List<Rutina>();
            var fechaHoy = DateTime.Today;

            // Personalizador usando delegate
            PersonalizadorRutina personalizador = (rutina, atletaObj) =>
            {
                // Ajustar según IMC y nivel
                var imc = atletaObj.CalcularIMC();
                if (imc > 30 && rutina is RutinaCardio cardio)
                {
                    // Reducir intensidad para atletas con sobrepeso
                    cardio.ActualizarCon(FabricaRutinas.CrearRutinaCardio(
                        cardio.Duracion - 5, "Baja", cardio.GrupoMuscular,
                        cardio.NombreAtleta, cardio.FechaRealizacion));
                }
                return rutina;
            };

            switch (atleta.Nivel.ToLower())
            {
                case "principiante":
                    if (tipoRutina.ToLower() == "fuerza")
                    {
                        rutinas.Add(personalizador(FabricaRutinas.CrearRutinaFuerza(
                            25, "Baja", grupoMuscular, atleta.Nombre, fechaHoy, 2, 15, 0), atleta));
                    }
                    else
                    {
                        rutinas.Add(personalizador(FabricaRutinas.CrearRutinaCardio(
                            15, "Baja", "Cardio", atleta.Nombre, fechaHoy, "Caminata", 1.5, 110), atleta));
                    }
                    break;

                case "intermedio":
                    if (tipoRutina.ToLower() == "fuerza")
                    {
                        rutinas.Add(personalizador(FabricaRutinas.CrearRutinaFuerza(
                            40, "Media", grupoMuscular, atleta.Nombre, fechaHoy, 3, 12, 20), atleta));
                    }
                    else
                    {
                        rutinas.Add(personalizador(FabricaRutinas.CrearRutinaCardio(
                            30, "Media", "Cardio", atleta.Nombre, fechaHoy, "Trote", 5.0, 140), atleta));
                    }
                    break;

                case "avanzado":
                    if (tipoRutina.ToLower() == "fuerza")
                    {
                        rutinas.Add(personalizador(FabricaRutinas.CrearRutinaFuerza(
                            60, "Alta", grupoMuscular, atleta.Nombre, fechaHoy, 4, 8, 50), atleta));
                    }
                    else
                    {
                        rutinas.Add(personalizador(FabricaRutinas.CrearRutinaCardio(
                            45, "Alta", "Cardio", atleta.Nombre, fechaHoy, "Correr", 8.0, 160), atleta));
                    }
                    break;
            }

            return rutinas;
        }

        /// <summary>
        /// Crea una rutina de rehabilitación para lesiones.
        /// </summary>
        public static Rutina CrearRutinaRehabilitacion(Atleta atleta, string tipoLesion)
        {
            var duracion = 20;
            var intensidad = "Baja";
            var grupoMuscular = tipoLesion.ToLower() switch
            {
                "rodilla" or "pierna" => "Piernas",
                "espalda" or "lumbar" => "Espalda",
                "hombro" or "brazo" => "Brazos",
                _ => "General"
            };

            return FabricaRutinas.CrearRutinaFuerza(
                duracion, intensidad, grupoMuscular, atleta.Nombre, DateTime.Today,
                series: 2, repeticiones: 20, peso: 0,
                lesiones: $"Rutina de rehabilitación para {tipoLesion}"
            );
        }
    }
}