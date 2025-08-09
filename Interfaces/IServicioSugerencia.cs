using AppEntrenamientoPersonal.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de sugerencias de entrenamientos.
    /// </summary>
    public interface IServicioSugerencias
    {
        List<string> ObtenerRutinasSugeridas(Atleta atleta); // Obtiene rutinas sugeridas para un atleta.
        List<string> ObtenerEjerciciosPorGrupo(string grupoMuscular, string nivel); // Obtiene ejercicios específicos por grupo muscular.
        List<string> ObtenerEjerciciosPorNivel(string nivel); // Obtiene ejercicios por nivel de entrenamiento.
    }
}
