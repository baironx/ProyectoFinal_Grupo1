using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz específica para repositorio de seguros médicos.
    /// </summary>
    public interface IRepositorioSeguros<T> : IRepositorio<T> where T : class
    {
        IEnumerable<T> BuscarPorNombreSeguro(string nombreSeguro); // Busca seguros por nombre del seguro.
        IEnumerable<T> BuscarPorMontoCubierto(decimal monto); // Busca seguros por monto cubierto.
        IEnumerable<T> BuscarPorMontoPaciente(decimal monto); // Busca seguros por monto del paciente.
        IEnumerable<T> BuscarPorAtleta(string nombreAtleta); // Busca seguros por atleta.
    }
}