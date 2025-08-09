using System;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Fabricas
{
    /// <summary>
    /// Fábrica para crear diferentes tipos de rutinas.
    /// Implementa el patrón Factory Method y cumple con Open/Closed Principle (OCP).
    /// </summary>
    public static class FabricaRutinas
    {
        /// <summary>
        /// Delegate para validación de parámetros.
        /// </summary>
        public delegate bool ValidadorParametros(params object[] parametros);

        /// <summary>
        /// Delegate para creación personalizada de rutinas.
        /// </summary>
        public delegate Rutina CreadorRutina(params object[] parametros);

        /// <summary>
        /// Crea una rutina del tipo especificado.
        /// </summary>
        public static Rutina CrearRutina(string tipo, int duracion, string intensidad, string grupoMuscular,
                                        string nombreAtleta, DateTime fechaRealizacion, DateTime? fechaVencimiento,
                                        string lesiones, SeguroMedico seguro = null!)
        {
            // Validación de parámetros usando delegate
            ValidadorParametros validador = (parametros) =>
            {
                return !string.IsNullOrWhiteSpace(tipo) &&
                       duracion > 0 &&
                       !string.IsNullOrWhiteSpace(intensidad) &&
                       !string.IsNullOrWhiteSpace(nombreAtleta);
            };

            if (!validador(tipo, duracion, intensidad, nombreAtleta))
            {
                throw new ArgumentException("Parámetros inválidos para crear la rutina");
            }

            // Factory Method pattern con delegates
            CreadorRutina creador = tipo.ToLower() switch
            {
                "fuerza" => (parametros) => new RutinaFuerza(
                    duracion, intensidad, grupoMuscular, nombreAtleta,
                    fechaRealizacion, fechaVencimiento, lesiones ?? string.Empty, seguro),

                "cardio" => (parametros) => new RutinaCardio(
                    duracion, intensidad, grupoMuscular, nombreAtleta,
                    fechaRealizacion, fechaVencimiento, lesiones ?? string.Empty, seguro),

                _ => throw new ArgumentException($"Tipo de rutina '{tipo}' no soportado")
            };

            return creador();
        }

        /// <summary>
        /// Crea una rutina de fuerza con parámetros específicos.
        /// </summary>
        public static RutinaFuerza CrearRutinaFuerza(int duracion, string intensidad, string grupoMuscular,
                                                     string nombreAtleta, DateTime fechaRealizacion,
                                                     int series = 3, int repeticiones = 10, double peso = 0,
                                                     DateTime? fechaVencimiento = null, string lesiones = "",
                                                     SeguroMedico seguro = null!)
        {
            return new RutinaFuerza(duracion, intensidad, grupoMuscular, nombreAtleta,
                                   fechaRealizacion, fechaVencimiento, lesiones, seguro,
                                   series, repeticiones, peso);
        }

        /// <summary>
        /// Crea una rutina de cardio con parámetros específicos.
        /// </summary>
        public static RutinaCardio CrearRutinaCardio(int duracion, string intensidad, string grupoMuscular,
                                                     string nombreAtleta, DateTime fechaRealizacion,
                                                     string tipoCardio = "General", double distancia = 0,
                                                     int frecuenciaCardiaca = 0, DateTime? fechaVencimiento = null,
                                                     string lesiones = "", SeguroMedico seguro = null!)
        {
            return new RutinaCardio(duracion, intensidad, grupoMuscular, nombreAtleta,
                                   fechaRealizacion, fechaVencimiento, lesiones, seguro,
                                   distancia, frecuenciaCardiaca, tipoCardio);
        }

        /// <summary>
        /// Obtiene los tipos de rutina soportados.
        /// </summary>
        public static string[] ObtenerTiposSoportados()
        {
            return new[] { "Fuerza", "Cardio" };
        }

        /// <summary>
        /// Verifica si un tipo de rutina es soportado.
        /// </summary>
        public static bool EsTipoSoportado(string tipo)
        {
            return Array.Exists(ObtenerTiposSoportados(),
                               t => t.Equals(tipo, StringComparison.OrdinalIgnoreCase));
        }
    }
}