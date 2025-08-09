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
    public class ServicioSugerenciaAvanzado : IServicioSugerencias
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
        public ServicioSugerenciaAvanzado()
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

        #region Métodos de Generación de Rutinas

        /// <summary>
        /// Genera rutinas base según el nivel del atleta.
        /// </summary>
        private List<string> GenerarRutinasPorNivel(string nivel, string objetivos)
        {
            return nivel switch
            {
                "principiante" => GenerarRutinasPrincipiante(objetivos),
                "intermedio" => GenerarRutinasIntermedio(objetivos),
                "avanzado" => GenerarRutinasAvanzado(objetivos),
                _ => GenerarRutinasGenerales(objetivos)
            };
        }

        private List<string> GenerarRutinasPrincipiante(string objetivos)
        {
            var rutinas = new List<string>();

            if (objetivos.Contains("fuerza") || objetivos.Contains("muscular"))
            {
                rutinas.AddRange(new[]
                {
                    "Fuerza - 25 min - Baja - Pecho (Flexiones de rodillas 2x12)",
                    "Fuerza - 20 min - Baja - Piernas (Sentadillas con peso corporal 2x15)",
                    "Fuerza - 15 min - Baja - Brazos (Flexiones de brazos asistidas 2x10)"
                });
            }

            if (objetivos.Contains("cardio") || objetivos.Contains("resistencia") || objetivos.Contains("peso"))
            {
                rutinas.AddRange(new[]
                {
                    "Cardio - 15 min - Baja - Caminata (Caminata rápida en terreno plano)",
                    "Cardio - 20 min - Baja - Bicicleta (Bicicleta estática ritmo suave)",
                    "Cardio - 10 min - Baja - Escaleras (Subir y bajar escaleras lentamente)"
                });
            }

            rutinas.Add("Flexibilidad - 10 min - Baja - Estiramiento (Rutina de estiramiento básica)");
            return rutinas;
        }

        private List<string> GenerarRutinasIntermedio(string objetivos)
        {
            var rutinas = new List<string>();

            if (objetivos.Contains("fuerza") || objetivos.Contains("muscular"))
            {
                rutinas.AddRange(new[]
                {
                    "Fuerza - 40 min - Media - Pecho (Press de banca 3x10, Fondos 3x12)",
                    "Fuerza - 45 min - Media - Piernas (Sentadillas con peso 3x12, Peso muerto 3x10)",
                    "Fuerza - 35 min - Media - Espalda (Dominadas asistidas 3x8, Remo con mancuernas 3x12)"
                });
            }

            if (objetivos.Contains("cardio") || objetivos.Contains("resistencia"))
            {
                rutinas.AddRange(new[]
                {
                    "Cardio - 30 min - Media - Intervalos (5 min calentamiento + 20 min intervalos + 5 min enfriamiento)",
                    "Cardio - 35 min - Media - Trote (Trote continuo ritmo moderado)",
                    "Cardio - 25 min - Media - HIIT (Entrenamiento intervalos alta intensidad)"
                });
            }

            if (objetivos.Contains("peso"))
            {
                rutinas.Add("Circuito - 30 min - Media - Quema grasa (6 estaciones x 45 seg trabajo / 15 seg descanso)");
            }

            return rutinas;
        }

        private List<string> GenerarRutinasAvanzado(string objetivos)
        {
            var rutinas = new List<string>();

            if (objetivos.Contains("fuerza") || objetivos.Contains("muscular"))
            {
                rutinas.AddRange(new[]
                {
                    "Fuerza - 60 min - Alta - Pecho (Press banca 4x6, Press inclinado 4x8, Fondos lastrados 3x10)",
                    "Fuerza - 70 min - Alta - Piernas (Sentadilla profunda 5x5, Peso muerto 4x6, Prensa 4x12)",
                    "Fuerza - 55 min - Alta - Espalda (Dominadas lastradas 4x6, Remo con barra 4x8, Jalones 3x10)"
                });
            }

            if (objetivos.Contains("cardio") || objetivos.Contains("resistencia"))
            {
                rutinas.AddRange(new[]
                {
                    "Cardio - 45 min - Alta - Resistencia (Carrera continua ritmo competitivo)",
                    "Cardio - 30 min - Alta - HIIT Avanzado (Sprints máximos + recuperación activa)",
                    "Cardio - 60 min - Media - Volumen (Entrenamiento aeróbico extenso)"
                });
            }

            if (objetivos.Contains("explosividad") || objetivos.Contains("potencia"))
            {
                rutinas.Add("Pliométrico - 40 min - Alta - Potencia (Saltos explosivos + levantamientos olímpicos)");
            }

            return rutinas;
        }

        private List<string> GenerarRutinasGenerales(string objetivos)
        {
            return new List<string>
            {
                "Fuerza - 35 min - Media - Cuerpo completo (Rutina funcional general)",
                "Cardio - 25 min - Media - Mixto (Combinación cardio + fuerza)",
                "Flexibilidad - 20 min - Baja - Movilidad (Yoga + estiramiento dinámico)",
                "Funcional - 30 min - Media - Equilibrio (Ejercicios de estabilidad + coordinación)"
            };
        }

        #endregion

        #region Generadores de Ejercicios Específicos

        /// <summary>
        /// Inicializa los generadores de ejercicios por grupo muscular.
        /// </summary>
        private Dictionary<string, GeneradorEjercicios> InicializarGeneradores()
        {
            return new Dictionary<string, GeneradorEjercicios>
            {
                ["pecho"] = GenerarEjerciciosPecho,
                ["espalda"] = GenerarEjerciciosEspalda,
                ["piernas"] = GenerarEjerciciosPiernas,
                ["brazos"] = GenerarEjerciciosBrazos,
                ["hombros"] = GenerarEjerciciosHombros,
                ["abdomen"] = GenerarEjerciciosAbdomen,
                ["cardio"] = GenerarEjerciciosCardio
            };
        }

        private List<string> GenerarEjerciciosPecho(string grupoMuscular, string nivel, string objetivos)
        {
            var ejercicios = new List<string>();

            switch (nivel.ToLower())
            {
                case "principiante":
                    ejercicios.AddRange(new[]
                    {
                        "Press de pecho con mancuernas - 3x12 (peso ligero)",
                        "Flexiones de rodillas - 3x10",
                        "Aperturas con mancuernas - 2x15 (peso muy ligero)",
                        "Flexiones en pared - 2x20"
                    });
                    break;

                case "intermedio":
                    ejercicios.AddRange(new[]
                    {
                        "Press de banca con barra - 3x10",
                        "Press inclinado con mancuernas - 3x12",
                        "Fondos en paralelas - 3x8",
                        "Aperturas inclinadas - 3x12",
                        "Flexiones diamante - 2x15"
                    });
                    break;

                case "avanzado":
                    ejercicios.AddRange(new[]
                    {
                        "Press de banca pesado - 4x6",
                        "Press inclinado con barra - 4x8",
                        "Fondos lastrados - 4x10",
                        "Aperturas con cables - 3x15",
                        "Press decline - 3x10",
                        "Flexiones pliométricas - 3x8"
                    });
                    break;
            }

            return ejercicios;
        }

        private List<string> GenerarEjerciciosEspalda(string grupoMuscular, string nivel, string objetivos)
        {
            var ejercicios = new List<string>();

            switch (nivel.ToLower())
            {
                case "principiante":
                    ejercicios.AddRange(new[]
                    {
                        "Remo con banda elástica - 3x15",
                        "Superman - 3x12",
                        "Jalón al pecho asistido - 3x10",
                        "Remo invertido - 2x8"
                    });
                    break;

                case "intermedio":
                    ejercicios.AddRange(new[]
                    {
                        "Dominadas asistidas - 3x8",
                        "Remo con barra - 3x10",
                        "Jalón al pecho - 3x12",
                        "Remo con mancuerna - 3x12 cada brazo",
                        "Peso muerto rumano - 3x10"
                    });
                    break;

                case "avanzado":
                    ejercicios.AddRange(new[]
                    {
                        "Dominadas lastradas - 4x6",
                        "Peso muerto convencional - 4x5",
                        "Remo con barra T - 4x8",
                        "Jalones con agarre amplio - 3x10",
                        "Remo en polea baja - 4x12",
                        "Muscle-ups - 3x5"
                    });
                    break;
            }

            return ejercicios;
        }

        private List<string> GenerarEjerciciosPiernas(string grupoMuscular, string nivel, string objetivos)
        {
            var ejercicios = new List<string>();

            switch (nivel.ToLower())
            {
                case "principiante":
                    ejercicios.AddRange(new[]
                    {
                        "Sentadilla con peso corporal - 3x15",
                        "Zancadas estáticas - 3x10 cada pierna",
                        "Elevación de talones - 3x20",
                        "Glute bridge - 3x15",
                        "Wall sit - 3x30 segundos"
                    });
                    break;

                case "intermedio":
                    ejercicios.AddRange(new[]
                    {
                        "Sentadilla con barra - 3x12",
                        "Peso muerto - 3x10",
                        "Zancadas con mancuernas - 3x12 cada pierna",
                        "Prensa de piernas - 3x15",
                        "Extensiones de cuádriceps - 3x15",
                        "Curl de femoral - 3x12"
                    });
                    break;

                case "avanzado":
                    ejercicios.AddRange(new[]
                    {
                        "Sentadilla profunda - 4x8",
                        "Peso muerto sumo - 4x6",
                        "Sentadilla búlgara - 4x10 cada pierna",
                        "Hip thrust con barra - 4x12",
                        "Sentadilla frontal - 3x10",
                        "Saltos en cajón - 4x8"
                    });
                    break;
            }

            return ejercicios;
        }

        private List<string> GenerarEjerciciosBrazos(string grupoMuscular, string nivel, string objetivos)
        {
            var ejercicios = new List<string>();

            switch (nivel.ToLower())
            {
                case "principiante":
                    ejercicios.AddRange(new[]
                    {
                        "Curl de bíceps con mancuernas - 3x12",
                        "Extensiones de tríceps - 3x12",
                        "Martillo con mancuernas - 3x10",
                        "Fondos en silla - 2x10"
                    });
                    break;

                case "intermedio":
                    ejercicios.AddRange(new[]
                    {
                        "Curl con barra - 3x10",
                        "Press francés - 3x12",
                        "Curl martillo - 3x12",
                        "Fondos en paralelas - 3x10",
                        "Curl concentrado - 3x10 cada brazo"
                    });
                    break;

                case "avanzado":
                    ejercicios.AddRange(new[]
                    {
                        "Curl con barra Z - 4x8",
                        "Press de banca agarre cerrado - 4x8",
                        "Curl 21s - 3 series",
                        "Fondos lastrados - 4x8",
                        "Curl con cables - 3x12",
                        "Extensiones sobre cabeza - 4x10"
                    });
                    break;
            }

            return ejercicios;
        }

        private List<string> GenerarEjerciciosHombros(string grupoMuscular, string nivel, string objetivos)
        {
            var ejercicios = new List<string>();

            switch (nivel.ToLower())
            {
                case "principiante":
                    ejercicios.AddRange(new[]
                    {
                        "Press militar con mancuernas sentado - 3x12",
                        "Elevaciones laterales - 3x15",
                        "Elevaciones frontales - 3x12",
                        "Pájaros - 3x15"
                    });
                    break;

                case "intermedio":
                    ejercicios.AddRange(new[]
                    {
                        "Press militar con barra - 3x10",
                        "Press Arnold - 3x12",
                        "Elevaciones laterales con cables - 3x15",
                        "Face pulls - 3x15",
                        "Upright rows - 3x12"
                    });
                    break;

                case "avanzado":
                    ejercicios.AddRange(new[]
                    {
                        "Press militar estricto - 4x6",
                        "Press tras nuca - 4x8",
                        "Elevaciones laterales 21s - 3 series",
                        "Handstand push-ups - 3x8",
                        "Dislocaciones con banda - 3x20",
                        "Press de hombros unilateral - 3x8 cada lado"
                    });
                    break;
            }

            return ejercicios;
        }

        private List<string> GenerarEjerciciosAbdomen(string grupoMuscular, string nivel, string objetivos)
        {
            var ejercicios = new List<string>();

            switch (nivel.ToLower())
            {
                case "principiante":
                    ejercicios.AddRange(new[]
                    {
                        "Crunches básicos - 3x15",
                        "Plancha - 3x30 segundos",
                        "Bicicleta - 3x20",
                        "Dead bug - 3x10 cada lado"
                    });
                    break;

                case "intermedio":
                    ejercicios.AddRange(new[]
                    {
                        "Crunches con peso - 3x20",
                        "Plancha lateral - 3x45 segundos cada lado",
                        "Mountain climbers - 3x30",
                        "Russian twists - 3x25",
                        "Leg raises - 3x15"
                    });
                    break;

                case "avanzado":
                    ejercicios.AddRange(new[]
                    {
                        "Dragon flags - 3x8",
                        "Plancha con elevación de piernas - 3x60 segundos",
                        "V-ups - 4x15",
                        "Hanging leg raises - 4x12",
                        "Ab wheel rollouts - 3x10",
                        "L-sits - 3x30 segundos"
                    });
                    break;
            }

            return ejercicios;
        }

        private List<string> GenerarEjerciciosCardio(string grupoMuscular, string nivel, string objetivos)
        {
            var ejercicios = new List<string>();

            switch (nivel.ToLower())
            {
                case "principiante":
                    ejercicios.AddRange(new[]
                    {
                        "Caminata rápida - 20-30 minutos",
                        "Bicicleta estática - 15-20 minutos",
                        "Natación suave - 15-20 minutos",
                        "Subir escaleras - 10-15 minutos"
                    });
                    break;

                case "intermedio":
                    ejercicios.AddRange(new[]
                    {
                        "Trote - 25-35 minutos",
                        "Intervalos en bicicleta - 20-30 minutos",
                        "Elíptica - 25-30 minutos",
                        "Circuito cardio - 20-25 minutos",
                        "Natación con intervalos - 25-30 minutos"
                    });
                    break;

                case "avanzado":
                    ejercicios.AddRange(new[]
                    {
                        "Sprints - 15-20 series de 100m",
                        "HIIT extremo - 20-25 minutos",
                        "Carrera de tempo - 35-45 minutos",
                        "Crosstraining - 30-40 minutos",
                        "Burpees - 10 series de 30 segundos",
                        "Battle ropes - 15 series de 45 segundos"
                    });
                    break;
            }

            return ejercicios;
        }

        #endregion

        #region Métodos de Personalización

        /// <summary>
        /// Personaliza una rutina según las características del atleta.
        /// </summary>
        private string PersonalizarRutinaPorAtleta(string rutinaBase, Atleta atleta)
        {
            var rutina = rutinaBase;

            // Ajustar por IMC
            var imc = atleta.CalcularIMC();
            if (imc > 30)
            {
                rutina = rutina.Replace("Alta", "Media").Replace("Media", "Baja");
                rutina += " (Adaptado para sobrepeso)";
            }
            else if (imc < 18.5)
            {
                rutina += " (Enfoque en ganancia de masa)";
            }

            // Ajustar por objetivos específicos
            if (atleta.Objetivos.Contains("pérdida", StringComparison.OrdinalIgnoreCase) ||
                atleta.Objetivos.Contains("peso", StringComparison.OrdinalIgnoreCase))
            {
                if (rutina.Contains("Cardio"))
                {
                    rutina += " + 5 min extra quema grasa";
                }
            }

            if (atleta.Objetivos.Contains("fuerza", StringComparison.OrdinalIgnoreCase))
            {
                if (rutina.Contains("Fuerza"))
                {
                    rutina += " + series adicionales";
                }
            }

            // Ajustar por nivel
            if (atleta.Nivel.Equals("Avanzado", StringComparison.OrdinalIgnoreCase))
            {
                rutina += " (Progresión avanzada)";
            }

            return rutina;
        }

        /// <summary>
        /// Obtiene ejercicios genéricos cuando no hay un generador específico.
        /// </summary>
        private List<string> ObtenerEjerciciosGenericos(string grupoMuscular, string nivel)
        {
            return new List<string>
            {
                $"Ejercicio básico de {grupoMuscular} - 3x12",
                $"Ejercicio intermedio de {grupoMuscular} - 3x10",
                $"Ejercicio de estabilización {grupoMuscular} - 2x15"
            };
        }

        /// <summary>
        /// Inicializa la base de datos de ejercicios.
        /// </summary>
        private Dictionary<string, Dictionary<string, List<string>>> InicializarBaseEjercicios()
        {
            // Esta sería una base de datos más completa en un sistema real
            return new Dictionary<string, Dictionary<string, List<string>>>();
        }

        #endregion

        #region Métodos de Extensión

        /// <summary>
        /// Genera programa de ejercicios progresivo.
        /// </summary>
        public Dictionary<int, List<string>> GenerarProgramaProgresivo(Atleta atleta, int semanas)
        {
            var programa = new Dictionary<int, List<string>>();

            for (int semana = 1; semana <= semanas; semana++)
            {
                var rutinasSemanales = ObtenerRutinasSugeridas(atleta);

                // Aplicar progresión
                rutinasSemanales = rutinasSemanales.Select(r => AplicarProgresion(r, semana)).ToList();

                programa[semana] = rutinasSemanales;
            }

            return programa;
        }

        /// <summary>
        /// Aplica progresión a una rutina basada en la semana.
        /// </summary>
        private string AplicarProgresion(string rutina, int semana)
        {
            if (semana <= 2) return rutina + " (Fase adaptación)";
            if (semana <= 4) return rutina + " (Fase desarrollo)";
            if (semana <= 6) return rutina + " (Fase intensificación)";
            return rutina + " (Fase especialización)";
        }

        #endregion
    }
}