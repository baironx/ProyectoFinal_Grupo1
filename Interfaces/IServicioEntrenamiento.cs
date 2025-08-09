using AppEntrenamientoPersonal.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de análisis de entrenamiento.
    /// </summary>
    public interface IServicioEntrenamiento
    {
        EstadisticasAtleta GenerarEstadisticas(string nombreAtleta); // Genera estadísticas de entrenamiento para un atleta.
        IEnumerable<Rutina> ObtenerLesionesUltimoMes(string nombreAtleta); // Obtiene lesiones del último mes para un atleta.
        IEnumerable<IGrouping<string, Rutina>> ObtenerTop3LesionesTrimestre(string nombreAtleta); // Obtiene las 3 lesiones más frecuentes del último trimestre.
        string GenerarAnalisisSeguros(string nombreAtleta); // Genera análisis de seguros médicos para un atleta.
    }
}
