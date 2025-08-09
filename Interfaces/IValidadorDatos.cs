using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz genérica para validadores de datos.
    /// </summary>
    public interface IValidadorDatos<T>
    {
        bool Validar(T objeto); // Valida un objeto.
        IEnumerable<string> ObtenerErrores(T objeto); // Obtiene los errores de validación de un objeto.
    }
}
