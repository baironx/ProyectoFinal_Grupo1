using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Entidades
{
    internal class RutinaCardio
    {
    }
}



public class RutinaCardio : Rutina
    {
        public RutinaCardio(int duracion, string intensidad, string grupoMuscular, string nombreAtleta, DateTime fechaRealizacion, DateTime? fechaVencimiento, string lesionesPostEntrenamiento)
            : base("Cardio", duracion, intensidad, grupoMuscular, nombreAtleta, fechaRealizacion, fechaVencimiento, lesionesPostEntrenamiento) { }
 
        /// <summary>
<<<<<<< HEAD
        /// Implementación específica de cómo se describe una rutina de cardio.
        /// Sobrescribe el método abstracto de la clase base.
=======
        /// ImplementaciÃ³n especÃ­fica de cÃ³mo se describe una rutina de cardio.
        /// Sobrescribe el mÃ©todo abstracto de la clase base.
>>>>>>> f6641db85a1d75dcb5e8b8757d3177e790129063
        /// </summary>
        public override string Describir()
        {
            return $"[Cardio] {Duracion} min - Intensidad: {Intensidad} - Grupo: {GrupoMuscular} - Realizada: {FechaRealizacion.ToShortDateString()}";
        }
    }
