using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Grupo1_AppEntrenamiento.Entidades
{
    internal class Rutina
    {
    }
}



  public abstract class Rutina : IRutinaAccionable   // <-- Implementa la interfaz

  { 

      public string Tipo { get; set; }

      public int Duracion { get; set; }

      public string Intensidad { get; set; }

      public string GrupoMuscular { get; set; }

      public string NombreAtleta { get; set; }

      public DateTime FechaRealizacion { get; set; } // Nuevo: Fecha en que se realizó la rutina

      public DateTime? FechaVencimiento { get; set; } // Nuevo: Fecha en que vence la rutina (puede ser nula)

      public string LesionesPostEntrenamiento { get; set; } // Nuevo: Lesiones producidas post-entrenamiento
 
      /// <summary>

      /// Constructor de una nueva instancia de la clase Rutina.

      /// </summary>

      public Rutina(string tipo, int duracion, string intensidad, string grupoMuscular, string nombreAtleta, DateTime fechaRealizacion, DateTime? fechaVencimiento, string lesionesPostEntrenamiento)

      {

          this.Tipo = tipo;

          this.Duracion = duracion;

          this.Intensidad = intensidad;

          this.GrupoMuscular = grupoMuscular;

          this.NombreAtleta = nombreAtleta;

          this.FechaRealizacion = fechaRealizacion;

          this.FechaVencimiento = fechaVencimiento;

          this.LesionesPostEntrenamiento = lesionesPostEntrenamiento;

      }
 
      /// <summary>

      /// Método abstracto que debe ser implementado por cada tipo específico de rutina.

      /// Define cómo se describe cada rutina particular. (Parte de IRutinaAccionable)

      /// </summary>

      public abstract string Describir(); // <-- Satisface el contrato de la interfaz
 
      /// <summary>

      /// Representación de texto general de cualquier rutina.

      /// </summary>

      public override string ToString()

      {

          string vencimiento = FechaVencimiento.HasValue ? $" - Vence: {FechaVencimiento.Value.ToShortDateString()}" : "";

          string lesiones = !string.IsNullOrEmpty(LesionesPostEntrenamiento) ? $" - Lesiones: {LesionesPostEntrenamiento}" : "";

          return $"{Tipo} - {Duracion} min - {Intensidad} - Músculo: {GrupoMuscular} - Atleta: {NombreAtleta} - Realizada: {FechaRealizacion.ToShortDateString()}{vencimiento}{lesiones}";

      }

  }
 
