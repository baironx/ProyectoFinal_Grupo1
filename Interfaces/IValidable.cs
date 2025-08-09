using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz que define el contrato para objetos que pueden ser validados.
    /// </summary>
    public interface IValidable
    {
        bool EsValido(); // Valida el estado actual del objeto.
        IEnumerable<string> ObtenerErroresValidacion(); // Obtiene los errores de validación del objeto.
    }
}