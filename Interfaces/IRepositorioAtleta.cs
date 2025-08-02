using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Interfaces
{
    internal class IRepositorioAtleta
    {
    }
}


using System.Collections.Generic;
using AppEntrenamientoPersonal.Entidades;

namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IRepositorioAtleta
    {
        void Agregar(Atleta atleta);
        Atleta BuscarPorNombre(string nombre);
        IEnumerable<Atleta> ObtenerTodos();
    }
}

