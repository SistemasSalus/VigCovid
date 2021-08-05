namespace VigCovidApp.ViewModels
{
    public class ReporteExcelManualViewModel
    {
        public string Trabajador { get; set; }
        public string Dni { get; set; }
        public string Sede { get; set; }
        public string Edad { get; set; }
        public string FechaIngreso { get; set; }
        public string ModoIngreso { get; set; }
        public string EstadoActual { get; set; }
        public string Medico { get; set; }
    }

    public class StrJson
    {
        public string str { get; set; }
    }
}