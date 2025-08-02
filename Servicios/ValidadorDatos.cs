using System;
using System.Collections.Generic;
using System.Linq;
using AppEntrenamientoPersonal.Entidades;

namespace AppEntrenamientopersonal.Servicios
{
    /// <summary>
    /// Delegate para validaciones de rutina.
    /// </summary>
    public delegate bool RutinaValidadorDelegate(Rutina rutina);

    /// <summary>
    /// Clase para validaciones relacionadas con rutinas.
    /// </summary>
    public static class ValidadorDatos
    {
        private static string rutaArchivoAtletas = "atletas.txt";

        public static bool ValidarRutina(Rutina rutina)
        {
            if (!System.IO.File.Exists(rutaArchivoAtletas)) return false;

            var lines = System.IO.File.ReadAllLines(rutaArchivoAtletas);
            return lines.Any(line => !string.IsNullOrWhiteSpace(line) && line.StartsWith(rutina.NombreAtleta + ","));
        }
    }
}
