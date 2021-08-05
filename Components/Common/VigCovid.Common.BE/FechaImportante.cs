using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("FechaImportante")]
    public class FechaImportante
    {
        [Key]
        public int Id { get; set; }

        public bool? DM { get; set; }

        public int TrabajadorId { get; set; }

        public int? TipoRango { get; set; }

        public string Descripcion { get; set; }
        public DateTime? Fecha { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public string Diagnostico { get; set; }

    }
}