using System;

namespace VigCovidApp.ViewModels
{
    public class RegistrarExamen
    {
        public DateTime FechaExamen { get; set; }
        public int TrabajadorId { get; set; }
        public int TipoExamen { get; set; }
        public int ResultadoExamen { get; set; }
    }
}