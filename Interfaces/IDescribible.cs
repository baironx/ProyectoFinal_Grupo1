using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz que define el contrato para objetos que pueden ser descritos.
    /// Implementa el principio de Interface Segregation (ISP).
    /// </summary>
    public interface IDescribible
    {
        string Describir(); // Método que devuelve una descripción del objeto.
    }
}
