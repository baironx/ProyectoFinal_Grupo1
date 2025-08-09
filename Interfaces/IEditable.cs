using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para objetos que pueden ser editados.
    /// </summary>
    public interface IEditable<T>
    {
        void ActualizarCon(T nuevoValor); // Actualiza el objeto con nuevos valores.
    }
}
