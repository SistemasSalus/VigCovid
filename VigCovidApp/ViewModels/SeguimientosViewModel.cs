using System;

namespace VigCovidApp.ViewModels
{
    public class SeguimientosViewModel
    {
        public int SeguimientoId { get; set; }
        public int RegistroTrabajadorId { get; set; }
        public string Fecha { get; set; }
        public int ClasificacionId { get; set; }
        public string ProximoSeguimiento { get; set; }
        public string SensacionFiebre { get; set; }
        public string Tos { get; set; }
        public string DolorGarganta { get; set; }
        public string DificultadRespiratoria { get; set; }
        public string CongestionNasal { get; set; }
        public string Cefalea { get; set; }
        public string MalestarGeneral { get; set; }
        public string PerdidaOlfato { get; set; }
        public string Asintomatico { get; set; }
        public string Comentario { get; set; }
        public int ResultadoCovid19 { get; set; }
        public DateTime? FechaResultadoCovid19 { get; set; }
        public int TipoEstadoId { get; set; }
        public int NroSeguimiento { get; set; }

        public string HipertensionArterial { get; set; }
        public string HipertensionArterialNoControlada { get; set; }
        public string AsmaModeradoSevero { get; set; }
        public string Diabetes { get; set; }
        public string Mayor65 { get; set; }
        public string Cancer { get; set; }
        public string CardiovascularGrave { get; set; }
        public string ImcMayor40 { get; set; }
        public string RenalDialisis { get; set; }
        public string PulmonarCronica { get; set; }
        public string TratInmunosupresor { get; set; }

        public string CasoPositivo { get; set; }
        public string CasoSospechoso { get; set; }
        public string RinofaringitisAguda { get; set; }
        public string NeumoniaViral { get; set; }
        public string ContactoEnfermedades { get; set; }
        public string Aislamiento { get; set; }
        public string Otros { get; set; }
        public string OtrosComentar { get; set; }
        public string Recetamedica { get; set; }

        public string PAntigeno { get; set; }

        public string PAntigenos { get; set; }

        public string PAntigenos5 { get; set; }

        public string PulsoOximetro { get; set; }

        public int TipoDiagnostico { get; set; }

    }
}