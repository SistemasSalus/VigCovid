using System;

namespace VigCovidApp.ViewModels
{
    public class ActualizarSeguimientoViewModel
    {
        public int Id { get; set; }
        public int RegistroTrabajadorId { get; set; }
        public int ClasificacionId { get; set; }
        public bool? SensacionFiebre { get; set; }
        public bool? Tos { get; set; }
        public bool? DolorGarganta { get; set; }
        public bool? DificultadRespiratoria { get; set; }
        public bool? CongestionNasal { get; set; }
        public bool? Cefalea { get; set; }
        public bool? MalestarGeneral { get; set; }
        public bool? PerdidaOlfato { get; set; }
        public bool? Asintomatico { get; set; }
        public string Comentario { get; set; }
        public int ResultadoCovid19 { get; set; }

        //public DateTime FechaResultadoCovid19 { get; set; }
        public int TipoEstadoId { get; set; }

        public bool? HipertensionArterial { get; set; }
        public bool? HipertensionArterialNoControlada { get; set; }
        public bool? AsmaModeradoSevero { get; set; }
        public bool? Diabetes { get; set; }
        public bool? Mayor65 { get; set; }
        public bool? Cancer { get; set; }
        public bool? CardiovascularGrave { get; set; }
        public bool? ImcMayor40 { get; set; }
        public bool? RenalDialisis { get; set; }
        public bool? PulmonarCronica { get; set; }
        public bool? TratInmunosupresor { get; set; }

        public bool? CasoPositivo { get; set; }
        public bool? CasoSospechoso { get; set; }
        public bool? RinofaringitisAguda { get; set; }
        public bool? NeumoniaViral { get; set; }
        public bool? ContactoEnfermedades { get; set; }
        public bool? Aislamiento { get; set; }
        public bool? Otros { get; set; }
        public string OtrosComentar { get; set; }
        public DateTime? ProximoSeguimiento { get; set; }
        public string Recetamedica { get; set; }

        public bool? PAntigeno { get; set; }

        public bool? PAntigenos { get; set; }

        public bool? PAntigenos5 { get; set; }

        public bool? PulsoOximetro { get; set; }

        public int? TipoDiagnostico { get; set; }

    }
}