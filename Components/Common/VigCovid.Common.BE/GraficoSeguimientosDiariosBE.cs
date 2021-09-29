namespace VigCovid.Common.BE
{
    public class GraficoSeguimientosDiariosBE
    {
        public string dia { get; set; }
        //public int CasosDiariosAsintomaticos { get; set; }
        //public int CasosDiariosSospechosos { get; set; }
        //public int CasosDiariosConfirmadoSintomatico { get; set; }

        //public int CasosSintomatico { get; set; }
        //public int CasosContactoDirecto { get; set; }
        //public int CasosReingreso { get; set; }

        public int Cuarentena { get; set; }
        public int Hospitalizado { get; set; }
        public int Fallecido { get; set; }
        public int AislamientoLeve { get; set; }
        public int AislamientoModerado { get; set; }
        public int AltaEpidemiologica { get; set; }

        public int AislamientoSevero  { get; set; }
        public int AislamientoCasoAsintomatico { get; set; }
        public int AislamientoPostHospitalitario { get; set; }


}
}
