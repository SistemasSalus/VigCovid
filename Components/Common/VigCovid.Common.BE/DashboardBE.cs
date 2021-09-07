using System;
using System.Collections.Generic;

namespace VigCovid.Common.BE
{
    public class IndicadoresBE
    {
        public int TrabajadorEnSeguimiento { get; set; }
        public int TotalSeguimientoHoy { get; set; }
        public int SeguimientoPorCompletarHoy { get; set; }
        public int TotalAltasHoy { get; set; }
        public int AltasPorCompletarHoy { get; set; }
        public int HospitalizadoHoy { get; set; }
        public int TotalAltas { get; set; }
        public int ModeradoCriticosHoy { get; set; }
        public int Cuarentena { get; set; }

        public int Fallecidos { get; set; }

        public int CovidPositivoAcumulado { get; set; }
        public int TotalIgG { get; set; }
        public int TotalIgM { get; set; }
        public int TotalIgG_IgM { get; set; }

        
    }

    public class AltasBE
    {
        public int Hoy { get; set; }
        public int Dadas { get; set; }
        public int Total { get; set; }

        public int TotalSeguimientos { get; set; }



    }
}