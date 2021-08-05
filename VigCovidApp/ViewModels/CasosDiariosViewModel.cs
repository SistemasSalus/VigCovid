namespace VigCovidApp.ViewModels
{
    public class CasosDiariosViewModel
    {
        public string dia { get; set; }
        public int CasosDiariosAsintomaticos { get; set; }
        public int CasosDiariosSospechosos { get; set; }
        public int CasosDiariosConfirmadoSintomatico { get; set; }
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