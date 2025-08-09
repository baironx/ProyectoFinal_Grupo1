using AppEntrenamientoPersonal.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para gestor de seguros médicos.
    /// </summary>
    public interface IGestorSeguros
    {
        IEnumerable<SeguroMedico> ObtenerTodos(); // Obtiene todos los seguros.
        void Agregar(SeguroMedico seguro); // Agrega un nuevo seguro.
        void Actualizar(SeguroMedico seguroOriginal, SeguroMedico seguroActualizado); // Actualiza un seguro existente.
        void Eliminar(SeguroMedico seguro); // Elimina un seguro.
        IEnumerable<SeguroMedico> BuscarPorNombreSeguro(string nombreSeguro); // Busca seguros por nombre del seguro.
        IEnumerable<SeguroMedico> BuscarPorMontoCubierto(decimal monto); // Busca seguros por monto cubierto.
        IEnumerable<SeguroMedico> BuscarPorMontoPaciente(decimal monto); // Busca seguros por monto del paciente.
        IEnumerable<SeguroMedico> BuscarPorAtleta(string nombreAtleta); // Busca seguros por atleta.
    }
}