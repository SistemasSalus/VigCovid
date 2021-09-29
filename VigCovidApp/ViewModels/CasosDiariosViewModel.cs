namespace VigCovidApp.ViewModels
{
    public class CasosDiariosViewModel
    {
        public string dia { get; set; }
        public int CasosDiariosAsintomaticos { get; set; }
        public int CasosDiariosSospechosos { get; set; }
        public int CasosDiariosConfirmadoSintomatico { get; set; }
    }


    public class SeguimientosDiariosViewModel
    {
        public string dia { get; set; }
        public int Cuarentena { get; set; }
        public int Hospitalizado { get; set; }
        public int Fallecido { get; set; }
        public int AislamientoLeve { get; set; }
        public int AltaEpidemiologica { get; set; }
        public int AislamientoModerado { get; set; }
        public int AislamientoSevero { get; set; }
        public int AislamientoCasoAsintomatico { get; set; }
        public int AislamientoPostHospitalitario { get; set; }

    }


            












    public class CasosDiariosAsintomaticosVM
    {
        public int numero { get; set; }
    }

    public class CasosDiariosSospechososVM
    {
        public int numero { get; set; }
    }

    public class CasosDiariosConfirmadoSintomaticoVM
    {
        public int numero { get; set; }
    }
}