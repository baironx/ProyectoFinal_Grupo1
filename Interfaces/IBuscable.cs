using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Interfaces
{
    internal class IBuscable
    {
    }
}

namespace AppEntrenamientoPersonal.Interfaces
{
    public interface IBuscable
    {
        bool CumpleCriterio(string criterio);
    }
}

