using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Interfaces
{
    internal class IFabricaRutina
    {
    }
}


using AppEntrenamientoPersonal.Entidades;

namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IFabricaRutina
    {
        Rutina CrearRutina(string tipo, string nombreAtleta, int duracion, string intensidad, string grupoMuscular);
    }
}
