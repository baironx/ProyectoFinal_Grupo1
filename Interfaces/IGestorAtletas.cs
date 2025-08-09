using AppEntrenamientoPersonal.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para gestor de atletas (Application Service).
    /// </summary>
    public interface IGestorAtletas
    {
        IEnumerable<Atleta> ObtenerTodos(); // Obtiene todos los atletas.
        void Agregar(Atleta atleta); // Agrega un nuevo atleta.
        void Actualizar(Atleta atletaOriginal, Atleta atletaActualizado); // Actualiza un atleta existente.
        void Eliminar(Atleta atleta); // Elimina un atleta.
    }
}
