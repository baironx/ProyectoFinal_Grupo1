using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Entidades
{
    internal class RutinaFuerza
    {
    }
}



namespace AppEntrenamientoPersonal.Entidades
{
    /// <summary>
    /// Clase que representa una rutina de entrenamiento de fuerza.
    /// Hereda de la clase Rutina.
    /// </summary>
    public class RutinaFuerza : Rutina
    {
        public RutinaFuerza(int duracion, string intensidad, string grupoMuscular, string nombreAtleta, DateTime fechaRealizacion, DateTime? fechaVencimiento, string lesionesPostEntrenamiento)
            : base("Fuerza", duracion, intensidad, grupoMuscular, nombreAtleta, fechaRealizacion, fechaVencimiento, lesionesPostEntrenamiento) { }
 
        /// <summary>
        /// Implementación específica de cómo se describe una rutina de fuerza.
        /// Sobrescribe el método abstracto de la clase base.
        /// </summary>
        public override string Describir()
        {
            return $"[Fuerza] {Duracion} min - Intensidad: {Intensidad} - Grupo: {GrupoMuscular} - Realizada: {FechaRealizacion.ToShortDateString()}";
        }
    }
