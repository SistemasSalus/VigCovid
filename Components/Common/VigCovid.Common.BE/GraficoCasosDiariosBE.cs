namespace VigCovid.Common.BE
{
    public class GraficoCasosDiariosBE
    {
        public string dia { get; set; }
        public int CasosDiariosAsintomaticos { get; set; }
        public int CasosDiariosSospechosos { get; set; }
        public int CasosDiariosConfirmadoSintomatico { get; set; }
    }
}