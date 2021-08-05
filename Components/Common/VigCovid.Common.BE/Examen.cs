using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("Examen")]
    public class Examen
    {
        [Key]
        public int Id { get; set; }

        public int TrabajadorId { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoPrueba { get; set; }
        public int Resultado { get; set; }
    }
}