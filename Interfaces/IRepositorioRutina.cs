using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Interfaces
{
    internal class IRepositorioRutina
    {
    }
}



using System.Collections.Generic;
using AppEntrenamientoPersonal.Entidades;

namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IRepositorioRutina
    {
        void Agregar(Rutina rutina);
        Rutina BuscarPorNombreAtleta(string nombreAtleta);
        IEnumerable<Rutina> ObtenerTodas();
    }
}
