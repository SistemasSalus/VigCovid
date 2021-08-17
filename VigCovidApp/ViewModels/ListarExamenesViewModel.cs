using System;

namespace VigCovidApp.ViewModels
{
    public class ListarExamenesViewModel
    {
        public int IdExamen { get; set; }
        public int IdTrabajador { get; set; }
        public DateTime FechaExamen { get; set; }
        public string TipoPrueba { get; set; }
        public string Resultado { get; set; }
    }

}