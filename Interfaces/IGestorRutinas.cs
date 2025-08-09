using AppEntrenamientoPersonal.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para gestor de rutinas (Application Service).
    /// </summary>
    public interface IGestorRutinas
    {
        IEnumerable<Rutina> ObtenerTodos(); // Obtiene todas las rutinas.
        IEnumerable<Rutina> ObtenerPorAtleta(string nombreAtleta); // Obtiene rutinas por atleta.
        void Agregar(Rutina rutina); // Agrega una nueva rutina.
        void Actualizar(Rutina rutinaOriginal, Rutina rutinaActualizada); // Actualiza una rutina existente.
        void Eliminar(Rutina rutina); // Elimina una rutina.
        IEnumerable<Rutina> BuscarRutinas(string nombreAtleta, string termino); // Busca rutinas por término.
        IEnumerable<Rutina> BuscarPorRangoFechas(string nombreAtleta, DateTime fechaInicio, DateTime fechaFin); // Busca rutinas por rango de fechas.
        IEnumerable<Rutina> BuscarPorIntensidad(string nombreAtleta, string intensidad); // Busca rutinas por intensidad.
        IEnumerable<Rutina> BusquedaCombinada(string nombreAtleta, string tipo = null!, string intensidad = null!, string grupoMuscular = null!); // Realiza búsqueda combinada con múltiples criterios.
    }
}
