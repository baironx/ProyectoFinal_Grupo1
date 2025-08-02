using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Entidade
{
    public class  Atleta
    {
        
    }

}



namespace AppEntrenamientoPersonal.Entidades

{

    public interface IAtleta

    {

        string Nombre { get; }

        double Peso { get; }

        double Altura { get; }

        string Objetivos { get; }

        string Nivel { get; }

        string ToString();



    }

    public class Atleta : IAtleta

    {

        public string Nombre { get; set; }

        public double Peso { get; set; }

        public double Altura { get; set; }

        public string Objetivos { get; set; }

        public string Nivel { get; set; }

        public override string ToString()

        {

            return $"Atleta: {Nombre}\n" +

                   $"Peso: {Peso} kg\n" +

                   $"Altura: {Altura} m\n" +

                   $"Objetivos: {Objetivos}\n" +

                   $"Nivel: {Nivel}";

        }

    }

}
