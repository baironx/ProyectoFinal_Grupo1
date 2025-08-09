using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para objetos que pueden ser buscados por términos.
    /// </summary>
    public interface IBuscable
    {
        bool CoincideCon(string termino); // Determina si el objeto coincide con un término de búsqueda.
    }
}
