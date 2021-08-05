using System;

namespace VigCovid.Common.BE
{
    public class LineaTiempo
    {
        public DateTime Fecha { get; set; }
        public string DateLine { get; set; }
        public string DateDate { get; set; }
        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public int? TipoRango { get; set; }
        public string Descripcion { get; set; }

        public string Diagnostico { get; set; }

        public bool DM { get; set; }
    }
}