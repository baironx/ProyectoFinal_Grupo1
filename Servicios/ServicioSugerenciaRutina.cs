using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Servicios
{
    /// <summary>
    /// Servicio que proporciona sugerencias de rutinas y ejercicios personalizadas.
    /// Implementa el principio Single Responsibility (SRP) y utiliza delegates para extensibilidad.
    /// </summary>
    public class ServicioSugerenciaRutina : IServicioSugerencias
    {
        #region Delegates

        /// <summary>
        /// Delegate para generar ejercicios personalizados.
        /// </summary>
        public delegate List<string> GeneradorEjercicios(string grupoMuscular, string nivel, string objetivos);

        /// <summary>
        /// Delegate para personalizar rutinas según el atleta.
        /// </summary>
        public delegate string PersonalizadorRutina(string rutina, Atleta atleta);

        #endregion

        #region Campos Privados

        private readonly Dictionary<string, GeneradorEjercicios> _generadoresEjercicios;
        private readonly PersonalizadorRutina _personalizador;
        private readonly Dictionary<string, Dictionary<string, List<string>>> _baseEjercicios;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del servicio de sugerencias.
        /// </summary>
        public ServicioSugerenciaRutina()
        {
            _generadoresEjercicios = InicializarGeneradores();
            _personalizador = PersonalizarRutinaPorAtleta;
            _baseEjercicios = InicializarBaseEjercicios();
        }

        #endregion

        #region Implementación de IServicioSugerencias

        /// <summary>
        /// Obtiene rutinas sugeridas para un atleta.
        /// </summary>
        public List<string> ObtenerRutinasSugeridas(Atleta atleta)
        {
            if (atleta == null)
                throw new ArgumentNullException(nameof(atleta));

            var sugerencias = new List<string>();
            var nivel = atleta.Nivel.ToLower();
            var objetivos = atleta.Objetivos.ToLower();

            // Generar rutinas base según nivel
            var rutinasBase = GenerarRutinasPorNivel(nivel, objetivos);

            // Personalizar cada rutina usando delegate
            foreach (var rutina in rutinasBase)
            {
                var rutinaPersonalizada = _personalizador(rutina, atleta);
                sugerencias.Add(rutinaPersonalizada);
            }

            return sugerencias;
        }

        /// <summary>
        /// Obtiene ejercicios específicos por grupo muscular.
        /// </summary>
        public List<string> ObtenerEjerciciosPorGrupo(string grupoMuscular, string nivel)
        {
            if (string.IsNullOrWhiteSpace(grupoMuscular))
                return new List<string>();

            var grupoNormalizado = grupoMuscular.ToLower();

            if (_generadoresEjercicios.ContainsKey(grupoNormalizado))
            {
                var generador = _generadoresEjercicios[grupoNormalizado];
                return generador(grupoMuscular, nivel, "general");
            }

            return ObtenerEjerciciosGenericos(grupoMuscular, nivel);
        }

        /// <summary>
        /// Obtiene ejercicios por nivel de entrenamiento.
        /// </summary>
        public List<string> ObtenerEjerciciosPorNivel(string nivel)
        {
            var ejercicios = new List<string>();
            var gruposMusculares = new[] { "Pecho", "Espalda", "Piernas", "Brazos", "Hombros", "Abdomen" };

            foreach (var grupo in gruposMusculares)
            {
                var ejerciciosGrupo = ObtenerEjerciciosPorGrupo(grupo, nivel);
                ejercicios.AddRange(ejerciciosGrupo.Take(2)); // 2 ejercicios por grupo
            }

            return ejercicios;
        }

        #endregion

        #region Métodos Privados de Inicialización

        private Dictionary<string, GeneradorEjercicios> InicializarGeneradores()
        {
            return new Dictionary<string, GeneradorEjercicios>
            {
                ["pecho"] = (grupo, nivel, obj) => new List<string> { $"Press de pecho {nivel} - 3x10", $"Flexiones {nivel} - 3x12" },
                ["espalda"] = (grupo, nivel, obj) => new List<string> { $"Dominadas {nivel} - 3x8", $"Remo {nivel} - 3x10" },
                ["piernas"] = (grupo, nivel, obj) => new List<string> { $"Sentadillas {nivel} - 3x12", $"Zancadas {nivel} - 3x10" },
                ["brazos"] = (grupo, nivel, obj) => new List<string> { $"Curl bíceps {nivel} - 3x12", $"Extensiones tríceps {nivel} - 3x10" },
                ["hombros"] = (grupo, nivel, obj) => new List<string> { $"Press militar {nivel} - 3x10", $"Elevaciones laterales {nivel} - 3x12" },
                ["abdomen"] = (grupo, nivel, obj) => new List<string> { $"Crunches {nivel} - 3x15", $"Plancha {nivel} - 3x30seg" },
                ["cardio"] = (grupo, nivel, obj) => new List<string> { $"Carrera {nivel} - 20-30min", $"Bicicleta {nivel} - 25min" }
            };
        }

        private List<string> GenerarRutinasPorNivel(string nivel, string objetivos)
        {
            return nivel switch
            {
                "principiante" => new List<string> { "Fuerza básica - 25min", "Cardio suave - 15min" },
                "intermedio" => new List<string> { "Fuerza intermedia - 40min", "Cardio moderado - 30min" },
                "avanzado" => new List<string> { "Fuerza intensa - 60min", "Cardio intenso - 45min" },
                _ => new List<string> { "Rutina general - 30min" }
            };
        }

        private string PersonalizarRutinaPorAtleta(string rutina, Atleta atleta)
        {
            return $"{rutina} (Personalizada para {atleta.Nombre} - {atleta.Nivel})";
        }

        private List<string> ObtenerEjerciciosGenericos(string grupoMuscular, string nivel)
        {
            return new List<string> { $"Ejercicio {grupoMuscular} {nivel} - 3x12" };
        }

        private Dictionary<string, Dictionary<string, List<string>>> InicializarBaseEjercicios()
        {
            return new Dictionary<string, Dictionary<string, List<string>>>();
        }

        #endregion
    }
}
