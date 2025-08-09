using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Entidades
{
    /// <summary>
    /// Representa un deportista con detalles personales y objetivos de entrenamiento.
    /// Implementa múltiples interfaces siguiendo el principio de Interface Segregation (ISP).
    /// </summary>
    public sealed class Atleta : IDescribible, IValidable, IBuscable, IEditable<Atleta>
    {
        #region Propiedades

        public string Id { get; private set; }
        public string Nombre { get; private set; }
        public double Peso { get; private set; }
        public double Altura { get; private set; }
        public string Objetivos { get; private set; }
        public string Nivel { get; private set; }
        public DateTime FechaRegistro { get; private set; }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor principal de la clase Atleta.
        /// </summary>
        public Atleta(string nombre, double peso, double altura, string objetivos, string nivel)
        {
            Id = Guid.NewGuid().ToString();
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Peso = peso;
            Altura = altura;
            Objetivos = objetivos ?? throw new ArgumentNullException(nameof(objetivos));
            Nivel = nivel ?? throw new ArgumentNullException(nameof(nivel));
            FechaRegistro = DateTime.Today;

            if (!EsValido())
            {
                throw new ArgumentException($"Datos del atleta inválidos: {string.Join(", ", ObtenerErroresValidacion())}");
            }
        }

        /// <summary>
        /// Constructor para reconstruir atleta desde datos persistidos.
        /// </summary>
        public Atleta(string id, string nombre, double peso, double altura, string objetivos, string nivel, DateTime fechaRegistro)
        {
            Id = id ?? Guid.NewGuid().ToString();
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Peso = peso;
            Altura = altura;
            Objetivos = objetivos ?? throw new ArgumentNullException(nameof(objetivos));
            Nivel = nivel ?? throw new ArgumentNullException(nameof(nivel));
            FechaRegistro = fechaRegistro;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Calcula el Índice de Masa Corporal del atleta.
        /// </summary>
        public double CalcularIMC()
        {
            if (Altura <= 0) return 0;
            return Math.Round(Peso / (Altura * Altura), 2);
        }

        /// <summary>
        /// Determina la categoría de IMC del atleta.
        /// </summary>
        public string ObtenerCategoriaIMC()
        {
            var imc = CalcularIMC();
            return imc switch
            {
                < 18.5 => "Bajo peso",
                >= 18.5 and < 25 => "Peso normal",
                >= 25 and < 30 => "Sobrepeso",
                >= 30 => "Obesidad",
                _ => "No determinado"
            };
        }

        /// <summary>
        /// Verifica si el atleta es compatible con un tipo de entrenamiento.
        /// </summary>
        public bool EsCompatibleCon(string tipoEntrenamiento)
        {
            return tipoEntrenamiento?.ToLower() switch
            {
                "fuerza" => Objetivos.Contains("Fuerza", StringComparison.OrdinalIgnoreCase) ||
                           Objetivos.Contains("muscular", StringComparison.OrdinalIgnoreCase),
                "cardio" => Objetivos.Contains("Resistencia", StringComparison.OrdinalIgnoreCase) ||
                           Objetivos.Contains("peso", StringComparison.OrdinalIgnoreCase) ||
                           Objetivos.Contains("cardio", StringComparison.OrdinalIgnoreCase),
                _ => true
            };
        }

        #endregion

        #region Implementación de Interfaces

        /// <summary>
        /// Implementación de IDescribible.
        /// </summary>
        public string Describir()
        {
            return $"{Nombre} - {Peso} kg - {Altura} m - {Objetivos} - {Nivel} (IMC: {CalcularIMC()})";
        }

        /// <summary>
        /// Implementación de IValidable.
        /// </summary>
        public bool EsValido()
        {
            return !ObtenerErroresValidacion().Any();
        }

        /// <summary>
        /// Implementación de IValidable.
        /// </summary>
        public IEnumerable<string> ObtenerErroresValidacion()
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(Nombre))
                errores.Add("El nombre es requerido");

            if (Peso <= 0 || Peso > 500)
                errores.Add("El peso debe estar entre 1 y 500 kg");

            if (Altura <= 0 || Altura > 300)
                errores.Add("La altura debe estar entre 0.1 y 3 metros");

            if (string.IsNullOrWhiteSpace(Objetivos))
                errores.Add("Los objetivos son requeridos");

            if (string.IsNullOrWhiteSpace(Nivel))
                errores.Add("El nivel es requerido");

            var nivelesValidos = new[] { "Principiante", "Intermedio", "Avanzado" };
            if (!nivelesValidos.Contains(Nivel, StringComparer.OrdinalIgnoreCase))
                errores.Add("El nivel debe ser Principiante, Intermedio o Avanzado");

            return errores;
        }

        /// <summary>
        /// Implementación de IBuscable.
        /// </summary>
        public bool CoincideCon(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino)) return false;

            var terminoLower = termino.ToLower();
            return Nombre.ToLower().Contains(terminoLower) ||
                   Objetivos.ToLower().Contains(terminoLower) ||
                   Nivel.ToLower().Contains(terminoLower);
        }

        /// <summary>
        /// Implementación de IEditable.
        /// </summary>
        public void ActualizarCon(Atleta nuevoAtleta)
        {
            if (nuevoAtleta == null)
                throw new ArgumentNullException(nameof(nuevoAtleta));

            if (!nuevoAtleta.EsValido())
                throw new ArgumentException("Los nuevos datos del atleta son inválidos");

            Nombre = nuevoAtleta.Nombre;
            Peso = nuevoAtleta.Peso;
            Altura = nuevoAtleta.Altura;
            Objetivos = nuevoAtleta.Objetivos;
            Nivel = nuevoAtleta.Nivel;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return Describir();
        }

        public override bool Equals(object obj)
        {
            if (obj is Atleta otroAtleta)
            {
                return Id == otroAtleta.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}