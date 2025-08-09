using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Interfaces;

namespace AppEntrenamientoPersonal.Servicios
{
    /// <summary>
    /// Gestor de seguros médicos.
    /// </summary>
    public class GestorSeguros : IGestorSeguros
    {
        private readonly IRepositorioSeguros<SeguroMedico> _repositorio;

        public GestorSeguros(IRepositorioSeguros<SeguroMedico> repositorio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public IEnumerable<SeguroMedico> ObtenerTodos()
        {
            return _repositorio.ObtenerTodos();
        }

        public void Agregar(SeguroMedico seguro)
        {
            if (seguro == null)
                throw new ArgumentNullException(nameof(seguro));

            if (!seguro.EsValido())
            {
                var errores = seguro.ObtenerErroresValidacion();
                throw new ArgumentException($"Seguro inválido: {string.Join(", ", errores)}");
            }

            _repositorio.Agregar(seguro);
        }

        public void Actualizar(SeguroMedico seguroOriginal, SeguroMedico seguroActualizado)
        {
            if (seguroOriginal == null)
                throw new ArgumentNullException(nameof(seguroOriginal));
            if (seguroActualizado == null)
                throw new ArgumentNullException(nameof(seguroActualizado));

            seguroOriginal.ActualizarCon(seguroActualizado);
            _repositorio.Actualizar(seguroOriginal);
        }

        public void Eliminar(SeguroMedico seguro)
        {
            if (seguro == null)
                throw new ArgumentNullException(nameof(seguro));

            _repositorio.Eliminar(seguro);
        }

        public IEnumerable<SeguroMedico> BuscarPorNombreSeguro(string nombreSeguro)
        {
            return _repositorio.BuscarPorNombreSeguro(nombreSeguro);
        }

        public IEnumerable<SeguroMedico> BuscarPorMontoCubierto(decimal monto)
        {
            return _repositorio.BuscarPorMontoCubierto(monto);
        }

        public IEnumerable<SeguroMedico> BuscarPorMontoPaciente(decimal monto)
        {
            return _repositorio.BuscarPorMontoPaciente(monto);
        }

        public IEnumerable<SeguroMedico> BuscarPorAtleta(string nombreAtleta)
        {
            return _repositorio.BuscarPorAtleta(nombreAtleta);
        }
    }
}