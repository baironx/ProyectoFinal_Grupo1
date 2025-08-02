using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Interfaces
{
    internal class IFabricaRutinas
    {
    }
}



using System.Collections.Generic;
using AppEntrenamientoPersonal.Entidades;

namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IFabricaRutinas
    {
        List<Rutina> CrearRutinas(IEnumerable<(string tipo, string nombreAtleta, int duracion, string intensidad, string grupoMuscular)> datos);
    }
}
