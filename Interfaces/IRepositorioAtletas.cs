using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz específica para repositorio de atletas.
    /// </summary>
    public interface IRepositorioAtletas<T> : IRepositorio<T> where T : class
    {
        IEnumerable<T> BuscarPorNombre(string nombre); // Busca atletas por nombre.
        IEnumerable<T> BuscarPorNivel(string nivel); // Busca atletas por nivel de entrenamiento.
        IEnumerable<T> BuscarPorObjetivos(string objetivos); // Busca atletas por objetivos.
    }
}