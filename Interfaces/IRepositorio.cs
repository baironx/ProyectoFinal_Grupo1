using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz genérica que define el contrato para repositorios de datos.
    /// Implementa el principio de Dependency Inversion (DIP).
    /// </summary>
    public interface IRepositorio<T> where T : class
    {
        IEnumerable<T> ObtenerTodos(); // Obtiene todos los elementos del repositorio.
        T ObtenerPorId(string id); // Obtiene un elemento por su identificador.
        void Agregar(T elemento); // Agrega un nuevo elemento al repositorio.
        void Actualizar(T elemento); // Actualiza un elemento existente.
        void Eliminar(T elemento); // Elimina un elemento del repositorio.
        void GuardarCambios(); // Guarda los cambios en el almacenamiento persistente.
    }
}