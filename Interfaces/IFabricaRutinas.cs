using AppEntrenamientoPersonal.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para fábrica de rutinas.
    /// </summary>
    public interface IFabricaRutinas
    {
        Rutina CrearRutina(string tipo, params object[] parametros); // Crea una rutina del tipo especificado.
    }
}