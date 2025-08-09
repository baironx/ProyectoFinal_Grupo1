using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para objetos que tienen operaciones relacionadas con tiempo.
    /// </summary>
    public interface ICalculableTiempo
    {
        bool EstaVencido(); // Calcula si el objeto está vencido.
        int DiasRestantes(); // Calcula los días restantes hasta el vencimiento.
    }
}
