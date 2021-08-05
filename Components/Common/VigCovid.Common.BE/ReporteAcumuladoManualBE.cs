using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace VigCovid.Common.BE
{
    public class ReporteAcumuladoManualBE
    {
        public int RegistroTrabajadorId { get; set; }
        public string ApellidosNombres { get; set; }
        public string Idhmc { get; set; }
        public string Dni { get; set; }
        public int Edad { get; set; }
        public string FechaRegistro { get; set; }
        public string Telefono { get; set; }
        public int SedeId { get; set; }
        public string Sede { get; set; }
        public string ModoIngreso { get; set; }
        public string Puesto { get; set; }
        public string DivisionPersonal { get; set; }
        public string CentroCosteArea { get; set; }
        public string MotivoIngreso { get; set; }
        public string ViaIngreso { get; set; }
        public string EstadoActual { get; set; }
        public string FechaUltimoDiaTrabajo { get; set; }
        public string AntecedentePatologico { get; set; }
        public string TipoContacto { get; set; }
        public string NombreContacto { get; set; }
        //public string PrimerResultadoPr { get; set; }
        //public string FechaResultadoPr { get; set; }
        //public string PrimerResultadoPositivoPr { get; set; }
        //public string FechaResultadoPositivoPr { get; set; }
        //public string ResultadoPcr { get; set; }
        //public string FechaResultadoPcr { get; set; }

        public string TipoExamen { get; set; }
        public string FechaExamen { get; set; }
        public string ResultadoExamen { get; set; }
        public string FechaInicioSintomas { get; set; }
        public string FechanFinSintomas { get; set; }
        public string NroDiasSinSintomas { get; set; }
        public string InicioDescansoMedico { get; set; }
        public string DescTipoRango { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechanFin { get; set; }
        public string NroDiasDescansoMedico { get; set; }
        public string NroDiasSegundoDescansoMedico { get; set; }
        public string FechaUltimaAlta { get; set; }

        public string FechaAislaminetoCuarentena { get; set; }

        public string FechaPosibleAltaA { get; set; }
        public string NroDiasPosibleAltaA { get; set; }
        public string FechaPosibleAltaB { get; set; }
        public string NroDiasPosibleAltaB { get; set; }
        public string FechaPosibleAltaC { get; set; }
        public string NroDiasPosibleAltaC { get; set; }

        public string FechaPosibleAltaDD { get; set; }
        public string NroDiasPosibleAltaD { get; set; }

        public string FechaPosibleAltaEE { get; set; }
        public string NroDiasPosibleAltaE { get; set; }

        public string FechaPosibleAltaFF { get; set; }
        public string NroDiasPosibleAltaF { get; set; }




        public string FechaAltaMedica { get; set; }

        public string NroDiasAltaMedica { get; set; }
        public string Medico { get; set; }

        public int MedicoVigilaId { get; set; }
        public string MedicoVigila { get; set; }

        public int EmpresaId { get; set; }
    }
}