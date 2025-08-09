using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Entidades
{
    /// <summary>
    /// Representa un seguro médico aplicado para tratar lesiones post-entrenamiento.
    /// Implementa múltiples interfaces para funcionalidad completa.
    /// </summary>
    public sealed class SeguroMedico : IDescribible, IValidable, IBuscable, IEditable<SeguroMedico>
    {
        #region Propiedades

        public string Id { get; private set; }
        public string NombreSeguro { get; private set; }
        public decimal MontoCubierto { get; private set; }
        public decimal MontoPaciente { get; private set; }
        public string LesionTratada { get; private set; }
        public string DescripcionTratamiento { get; private set; }
        public string NombreAtleta { get; private set; }
        public DateTime FechaAplicacion { get; private set; }
        public DateTime? FechaFinalizacion { get; private set; }
        public EstadoSeguro Estado { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor principal para seguro médico.
        /// </summary>
        public SeguroMedico(string id, string nombreSeguro, decimal montoCubierto, decimal montoPaciente,
                           string lesionTratada, string descripcionTratamiento, string nombreAtleta,
                           DateTime fechaAplicacion, DateTime? fechaFinalizacion = null, EstadoSeguro estado = EstadoSeguro.Activo)
        {
            Id = id ?? Guid.NewGuid().ToString();
            NombreSeguro = nombreSeguro ?? throw new ArgumentNullException(nameof(nombreSeguro));
            MontoCubierto = montoCubierto;
            MontoPaciente = montoPaciente;
            LesionTratada = lesionTratada ?? throw new ArgumentNullException(nameof(lesionTratada));
            DescripcionTratamiento = descripcionTratamiento ?? string.Empty;
            NombreAtleta = nombreAtleta ?? throw new ArgumentNullException(nameof(nombreAtleta));
            FechaAplicacion = fechaAplicacion;
            FechaFinalizacion = fechaFinalizacion;
            Estado = estado;

            if (!EsValido())
            {
                throw new ArgumentException($"Datos del seguro médico inválidos: {string.Join(", ", ObtenerErroresValidacion())}");
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Calcula el monto total del tratamiento.
        /// </summary>
        public decimal CalcularMontoTotal()
        {
            return MontoCubierto + MontoPaciente;
        }

        /// <summary>
        /// Calcula el porcentaje cubierto por el seguro.
        /// </summary>
        public double CalcularPorcentajeCobertura()
        {
            var montoTotal = CalcularMontoTotal();
            return montoTotal > 0 ? (double)(MontoCubierto / montoTotal) * 100 : 0;
        }

        /// <summary>
        /// Calcula la duración del tratamiento en días.
        /// </summary>
        public int CalcularDuracionTratamiento()
        {
            var fechaFin = FechaFinalizacion ?? DateTime.Today;
            return (fechaFin - FechaAplicacion).Days;
        }

        /// <summary>
        /// Finaliza el tratamiento del seguro.
        /// </summary>
        public void FinalizarTratamiento()
        {
            FechaFinalizacion = DateTime.Today;
            Estado = EstadoSeguro.Finalizado;
        }

        /// <summary>
        /// Suspende temporalmente el seguro.
        /// </summary>
        public void SuspenderSeguro()
        {
            Estado = EstadoSeguro.Suspendido;
        }

        /// <summary>
        /// Reactiva el seguro si estaba suspendido.
        /// </summary>
        public void ReactivarSeguro()
        {
            if (Estado == EstadoSeguro.Suspendido)
            {
                Estado = EstadoSeguro.Activo;
            }
        }

        #endregion

        #region Implementación de Interfaces

        /// <summary>
        /// Implementación de IDescribible.
        /// </summary>
        public string Describir()
        {
            var duracion = CalcularDuracionTratamiento();
            var cobertura = CalcularPorcentajeCobertura();

            return $"{NombreSeguro} - {NombreAtleta} - Lesión: {LesionTratada} - " +
                   $"Total: ₡{CalcularMontoTotal():F2} (Cobertura: {cobertura:F1}%) - " +
                   $"Duración: {duracion} días - Estado: {Estado}";
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

            if (string.IsNullOrWhiteSpace(NombreSeguro))
                errores.Add("El nombre del seguro es requerido");

            if (MontoCubierto < 0)
                errores.Add("El monto cubierto no puede ser negativo");

            if (MontoPaciente < 0)
                errores.Add("El monto del paciente no puede ser negativo");

            if (MontoCubierto == 0 && MontoPaciente == 0)
                errores.Add("Al menos uno de los montos debe ser mayor a cero");

            if (string.IsNullOrWhiteSpace(LesionTratada))
                errores.Add("La lesión tratada es requerida");

            if (string.IsNullOrWhiteSpace(NombreAtleta))
                errores.Add("El nombre del atleta es requerido");

            if (FechaAplicacion > DateTime.Today)
                errores.Add("La fecha de aplicación no puede ser futura");

            if (FechaFinalizacion.HasValue && FechaFinalizacion.Value <= FechaAplicacion)
                errores.Add("La fecha de finalización debe ser posterior a la fecha de aplicación");

            return errores;
        }

        /// <summary>
        /// Implementación de IBuscable.
        /// </summary>
        public bool CoincideCon(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino)) return false;

            var terminoLower = termino.ToLower();
            return NombreSeguro.ToLower().Contains(terminoLower) ||
                   LesionTratada.ToLower().Contains(terminoLower) ||
                   NombreAtleta.ToLower().Contains(terminoLower) ||
                   DescripcionTratamiento.ToLower().Contains(terminoLower) ||
                   MontoCubierto.ToString().Contains(termino) ||
                   MontoPaciente.ToString().Contains(termino);
        }

        /// <summary>
        /// Implementación de IEditable.
        /// </summary>
        public void ActualizarCon(SeguroMedico nuevoSeguro)
        {
            if (nuevoSeguro == null)
                throw new ArgumentNullException(nameof(nuevoSeguro));

            if (!nuevoSeguro.EsValido())
                throw new ArgumentException("Los nuevos datos del seguro son inválidos");

            NombreSeguro = nuevoSeguro.NombreSeguro;
            MontoCubierto = nuevoSeguro.MontoCubierto;
            MontoPaciente = nuevoSeguro.MontoPaciente;
            LesionTratada = nuevoSeguro.LesionTratada;
            DescripcionTratamiento = nuevoSeguro.DescripcionTratamiento;
            FechaFinalizacion = nuevoSeguro.FechaFinalizacion;
            Estado = nuevoSeguro.Estado;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return Describir();
        }

        public override bool Equals(object obj)
        {
            if (obj is SeguroMedico otroSeguro)
            {
                return Id == otroSeguro.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Enumeración para los estados posibles de un seguro médico.
    /// </summary>
    public enum EstadoSeguro
    {
        Activo,
        Suspendido,
        Finalizado,
        Cancelado
    }
}
