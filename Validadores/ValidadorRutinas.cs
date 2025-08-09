using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Servicios
{
    /// <summary>
    /// Validador especializado para entidades Rutina.
    /// </summary>
    public class ValidadorRutinas : IValidadorDatos<Rutina>
    {
        #region Delegates

        public delegate bool ValidadorRutina(Rutina rutina);
        public delegate string GeneradorMensaje(Rutina rutina);

        #endregion

        #region Campos Privados

        private readonly Dictionary<string, ValidadorRutina> _validadores;
        private readonly Dictionary<string, GeneradorMensaje> _mensajes;
        private readonly HashSet<string> _intensidadesValidas;
        private readonly HashSet<string> _gruposMusculares;

        #endregion

        #region Constructor

        public ValidadorRutinas()
        {
            _intensidadesValidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Baja", "Media", "Alta"
            };

            _gruposMusculares = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Pecho", "Espalda", "Piernas", "Brazos", "Hombros", "Abdomen", "Cardio", "General"
            };

            _validadores = new Dictionary<string, ValidadorRutina>
            {
                ["duracion"] = rutina => rutina.Duracion > 0 && rutina.Duracion <= 480,
                ["intensidad"] = rutina => _intensidadesValidas.Contains(rutina.Intensidad),
                ["grupo"] = rutina => _gruposMusculares.Contains(rutina.GrupoMuscular),
                ["atleta"] = rutina => !string.IsNullOrWhiteSpace(rutina.NombreAtleta),
                ["fechas"] = rutina => ValidarFechas(rutina),
                ["coherencia"] = rutina => ValidarCoherencia(rutina)
            };

            _mensajes = new Dictionary<string, GeneradorMensaje>
            {
                ["duracion"] = rutina => $"Duración {rutina.Duracion} min fuera de rango (1-480 min)",
                ["intensidad"] = rutina => $"Intensidad '{rutina.Intensidad}' no válida",
                ["grupo"] = rutina => $"Grupo muscular '{rutina.GrupoMuscular}' no válido",
                ["atleta"] = rutina => "Nombre del atleta requerido",
                ["fechas"] = rutina => "Fechas inválidas o inconsistentes",
                ["coherencia"] = rutina => "Datos de rutina incoherentes"
            };
        }

        #endregion

        #region Implementación de IValidadorDatos<Rutina>

        public bool Validar(Rutina rutina)
        {
            if (rutina == null) return false;
            return _validadores.Values.All(validador => validador(rutina));
        }

        public IEnumerable<string> ObtenerErrores(Rutina rutina)
        {
            if (rutina == null)
            {
                yield return "La rutina no puede ser nula";
                yield break;
            }

            foreach (var kvp in _validadores)
            {
                if (!kvp.Value(rutina))
                {
                    yield return _mensajes[kvp.Key](rutina);
                }
            }
        }

        #endregion

        #region Métodos de Validación Específicos

        private bool ValidarFechas(Rutina rutina)
        {
            if (rutina.FechaRealizacion > DateTime.Today.AddDays(1))
                return false;

            if (rutina.FechaVencimiento.HasValue)
            {
                return rutina.FechaVencimiento.Value > rutina.FechaRealizacion;
            }

            return true;
        }

        private bool ValidarCoherencia(Rutina rutina)
        {
            // Validar coherencia entre tipo y grupo muscular
            if (rutina.Tipo == "Cardio" && rutina.GrupoMuscular != "Cardio" && rutina.GrupoMuscular != "General")
            {
                return false;
            }

            // Validar duración vs intensidad
            if (rutina.Intensidad == "Alta" && rutina.Duracion > 120)
            {
                return false; // Rutinas de alta intensidad no deberían ser muy largas
            }

            return true;
        }

        #endregion
    }
}