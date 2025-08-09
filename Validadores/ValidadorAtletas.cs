using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Servicios
{
    /// <summary>
    /// Validador especializado para entidades Atleta.
    /// Implementa el principio Single Responsibility (SRP) y utiliza delegates para validaciones personalizadas.
    /// </summary>
    public class ValidadorAtletas : IValidadorDatos<Atleta>
    {
        #region Delegates

        /// <summary>
        /// Delegate para validaciones personalizadas.
        /// </summary>
        public delegate bool ValidacionPersonalizada(Atleta atleta);

        /// <summary>
        /// Delegate para obtener mensaje de error de validación.
        /// </summary>
        public delegate string GeneradorMensajeError(Atleta atleta);

        #endregion

        #region Campos Privados

        private readonly Dictionary<string, ValidacionPersonalizada> _validaciones;
        private readonly Dictionary<string, GeneradorMensajeError> _mensajesError;
        private readonly HashSet<string> _nivelesValidos;
        private readonly Regex _regexNombre;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del validador de atletas.
        /// </summary>
        public ValidadorAtletas()
        {
            _nivelesValidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Principiante", "Intermedio", "Avanzado"
            };

            _regexNombre = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]{2,50}$", RegexOptions.Compiled);

            // Configurar validaciones con delegates
            _validaciones = new Dictionary<string, ValidacionPersonalizada>
            {
                ["nombre"] = atleta => ValidarNombre(atleta.Nombre),
                ["peso"] = atleta => ValidarPeso(atleta.Peso),
                ["altura"] = atleta => ValidarAltura(atleta.Altura),
                ["nivel"] = atleta => ValidarNivel(atleta.Nivel),
                ["objetivos"] = atleta => ValidarObjetivos(atleta.Objetivos),
                ["imc"] = atleta => ValidarIMC(atleta.CalcularIMC())
            };

            // Configurar mensajes de error
            _mensajesError = new Dictionary<string, GeneradorMensajeError>
            {
                ["nombre"] = atleta => $"Nombre '{atleta.Nombre}' no es válido. Debe contener solo letras y espacios (2-50 caracteres)",
                ["peso"] = atleta => $"Peso {atleta.Peso} kg está fuera del rango válido (10-500 kg)",
                ["altura"] = atleta => $"Altura {atleta.Altura} m está fuera del rango válido (0.5-2.5 m)",
                ["nivel"] = atleta => $"Nivel '{atleta.Nivel}' no es válido. Debe ser: {string.Join(", ", _nivelesValidos)}",
                ["objetivos"] = atleta => $"Objetivos '{atleta.Objetivos}' no pueden estar vacíos",
                ["imc"] = atleta => $"IMC {atleta.CalcularIMC():F1} indica valores extremos. Revisar peso y altura"
            };
        }

        #endregion

        #region Implementación de IValidadorDatos<Atleta>

        /// <summary>
        /// Valida el peso del atleta.
        /// </summary>
        private bool ValidarPeso(double peso)
        {
            return peso >= 10 && peso <= 500; // Rango realista para humanos
        }

        /// <summary>
        /// Valida la altura del atleta.
        /// </summary>
        private bool ValidarAltura(double altura)
        {
            return altura >= 0.5 && altura <= 2.5; // Rango realista para humanos
        }

        /// <summary>
        /// Valida el nivel del atleta.
        /// </summary>
        private bool ValidarNivel(string nivel)
        {
            return !string.IsNullOrWhiteSpace(nivel) && _nivelesValidos.Contains(nivel);
        }

        /// <summary>
        /// Valida los objetivos del atleta.
        /// </summary>
        private bool ValidarObjetivos(string objetivos)
        {
            return !string.IsNullOrWhiteSpace(objetivos) &&
                   objetivos.Length >= 3 &&
                   objetivos.Length <= 200;
        }

        /// <summary>
        /// Valida el IMC calculado.
        /// </summary>
        private bool ValidarIMC(double imc)
        {
            return imc >= 10 && imc <= 60; // Rango extremo pero posible
        }

        /// <summary>
        /// Verifica que el nombre no contenga palabras prohibidas.
        /// </summary>
        private bool ContienePalabrasProhibidas(string nombre)
        {
            var palabrasProhibidas = new[] { "admin", "test", "null", "undefined" };
            return palabrasProhibidas.Any(palabra =>
                nombre.Contains(palabra, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Métodos Adicionales

        /// <summary>
        /// Valida un atleta con validaciones personalizadas adicionales.
        /// </summary>
        /// <param name="atleta">Atleta a validar.</param>
        /// <param name="validacionesAdicionales">Validaciones adicionales.</param>
        /// <returns>True si pasa todas las validaciones.</returns>
        public bool ValidarConExtensiones(Atleta atleta, params ValidacionPersonalizada[] validacionesAdicionales)
        {
            if (!Validar(atleta)) return false;

            return validacionesAdicionales.All(validacion => validacion(atleta));
        }

        /// <summary>
        /// Valida consistencia entre datos del atleta.
        /// </summary>
        /// <param name="atleta">Atleta a validar.</param>
        /// <returns>Lista de advertencias de consistencia.</returns>
        public IEnumerable<string> ValidarConsistencia(Atleta atleta)
        {
            if (atleta == null) yield break;

            var imc = atleta.CalcularIMC();
            var nivel = atleta.Nivel.ToLower();
            var objetivos = atleta.Objetivos.ToLower();

            // Verificar consistencia IMC vs nivel
            if (imc > 30 && nivel == "avanzado")
            {
                yield return "ADVERTENCIA: IMC alto para nivel avanzado. Verificar datos.";
            }

            // Verificar consistencia objetivos vs características físicas
            if (objetivos.Contains("pérdida") && imc < 18.5)
            {
                yield return "ADVERTENCIA: Objetivo de pérdida de peso con IMC bajo.";
            }

            if (objetivos.Contains("ganancia") && imc > 25)
            {
                yield return "ADVERTENCIA: Objetivo de ganancia de peso con IMC alto.";
            }

            // Verificar edad implícita por peso y altura (estimación)
            if (atleta.Peso < 30 && atleta.Altura < 1.2)
            {
                yield return "ADVERTENCIA: Datos sugieren atleta menor de edad. Verificar permisos.";
            }
        }

        #endregion

        #region Implementación de IValidadorDatos<Atleta>

        /// <summary>
        /// Valida un atleta completo usando todas las reglas de negocio.
        /// </summary>
        /// <param name="atleta">El atleta a validar.</param>
        /// <returns>True si el atleta es válido; de lo contrario, false.</returns>
        public bool Validar(Atleta atleta)
        {
            if (atleta == null) return false;

            // Usa LINQ para verificar que todas las validaciones pasen
            return _validaciones.Values.All(validacion => validacion(atleta));
        }

        /// <summary>
        /// Obtiene todos los errores de validación de un atleta.
        /// </summary>
        /// <param name="atleta">El atleta a validar.</param>
        /// <returns>Una lista de mensajes de error.</returns>
        public IEnumerable<string> ObtenerErrores(Atleta atleta)
        {
            if (atleta == null)
            {
                yield return "El atleta no puede ser nulo.";
                yield break;
            }

            // Itera sobre las validaciones y devuelve los mensajes de error si fallan
            foreach (var validacion in _validaciones)
            {
                if (!validacion.Value(atleta))
                {
                    yield return _mensajesError[validacion.Key](atleta);
                }
            }
        }

        /// <summary>
        /// Valida el nombre del atleta.
        /// </summary>
        private bool ValidarNombre(string nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre) &&
                   _regexNombre.IsMatch(nombre) &&
                   !ContienePalabrasProhibidas(nombre);
        }

        #endregion
    }
}