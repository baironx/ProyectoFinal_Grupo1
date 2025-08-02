using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEntrenamientoPersonal.Entidades
{

}



namespace AppEntrenamientoPersonal.Entidades
{
    public class SeguroLesionesBasico : ISeguroLesiones
    {
        public string TipoSeguro { get; private set; }
        public decimal MontoCubierto { get; private set; }
        public decimal MontoCubiertoPorSeguro { get; private set; } // parametros de la clase seguro lesiones
        public decimal MontoPagadoPorPaciente { get; private set; }
        public string NombrePaciente { get; private set; }

        private List<string> lesionesReportadas = new();// lista de lesiones reportadas

        public SeguroLesionesBasico(string tipoSeguro, decimal montoCubierto)// constructor de la clase seguro lesiones
        {
            this.TipoSeguro = tipoSeguro;// inicializa los valores de la clase seguro lesiones
            this.MontoCubierto = montoCubierto;
            this.MontoCubiertoPorSeguro = 0;
            this.MontoPagadoPorPaciente = 0;
            this.NombrePaciente = string.Empty;//se pone en empty para que no haya un error de null
        }

        public decimal CalcularMontoCobertura(string lesion, decimal monto)// metodo que calcula el monto cubierto por el seguro
        
            // aqui  simplemente devolvemos el monto cubierto por el seguro.
            // auqi  asumimos que el monto cubierto es igual al monto del seguro.
            {
                return Math.Min(monto, MontoCubierto);
            }


        public void ReportarLesion(string lesion, decimal monto, decimal montoPagado, string nombrePaciente)// metodo que reporta una lesion y calcula el monto cubierto por el seguro
        {
            this.MontoCubiertoPorSeguro = CalcularMontoCobertura(lesion, monto);
            this.MontoPagadoPorPaciente = montoPagado;
            this.NombrePaciente = nombrePaciente;
            this.lesionesReportadas.Add(lesion);// agrega la lesion a la lista de lesiones reportadas
        }

        public IList<string> ObtenerLesionesReportadas()// metodo que devuelve una lista de lesiones reportadas
        {
            return lesionesReportadas.AsReadOnly();
        }
    }
    public class GestorSeguros : IBuscadorSeguros// clase que implementa la interfaz IBuscadorSeguros para gestionar y buscar sweguros de lesiones
    {
        private readonly IList<ISeguroLesiones> seguros;// lista de seguros de lesiones

        public GestorSeguros(IList<ISeguroLesiones> seguros)// constructor de la clase GestorSeguros que recibe una lista de seguros de lesiones
        {
            this.seguros = seguros;
        }

        public IList<ISeguroLesiones> Buscar(Func<ISeguroLesiones, bool> criterio)
        {
            return seguros.Where(criterio).ToList();
        }

        public IList<ISeguroLesiones> ObtenerSegurosConPagoAlto()
        {
            return seguros.Where(s => s.MontoPagadoPorPaciente > 10000).ToList(); // implemnetacion de LINQ para que busuqe tanto por nombres y  tipos de seguro
        }

        public IList<ISeguroLesiones> ObtenerSegurosConPagobajo()
        {
            return seguros.Where(s => s.MontoPagadoPorPaciente < 10000).ToList();
        }

        public IList<ISeguroLesiones> ObtenerSegurosPorNombre(string NombrePaciente)
        {
            return seguros.Where(s => s.NombrePaciente == NombrePaciente).ToList();
        }
    }



    //ietfaces que definen contratos para las funcionalidades de seguros de lesiones y búsqueda de seguros


    public interface ISeguroLesiones// interfaz que define un contrato para los seguros de lesiones
    {
        string TipoSeguro { get; }
        decimal MontoCubierto { get; }
        decimal MontoCubiertoPorSeguro { get; }
        decimal MontoPagadoPorPaciente { get; }
        string NombrePaciente { get; }
        decimal CalcularMontoCobertura(string lesion, decimal monto);
        void ReportarLesion(string lesion, decimal monto, decimal montoPagado, string nombrePaciente);
        IList<string> ObtenerLesionesReportadas();
    }

    public interface IBuscadorSeguros// interfaz que define un contrato para buscar seguros de lesiones
    {
        IList<ISeguroLesiones> Buscar(Func<ISeguroLesiones, bool> criterio);
    }
}
