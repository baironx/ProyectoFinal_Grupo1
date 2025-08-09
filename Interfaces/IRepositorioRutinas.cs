using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz específica para repositorio de rutinas.
    /// </summary>
    public interface IRepositorioRutinas<T> : IRepositorio<T> where T : class
    {
        IEnumerable<T> ObtenerPorAtleta(string nombreAtleta); // Obtiene rutinas por nombre de atleta.
        IEnumerable<T> BuscarPorTipo(string tipo); // Busca rutinas por tipo.
        IEnumerable<T> BuscarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin); // Busca rutinas por rango de fechas.
        IEnumerable<T> BuscarConLesiones(); // Busca rutinas con lesiones post-entrenamiento.
    }
}
