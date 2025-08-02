using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Interfaces
{
    internal class IServicioSugerencia
    {
    }
}



namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IServicioSugerencia
    {
        string GenerarSugerenciaPersonalizada(string nombreAtleta);
    }
}
