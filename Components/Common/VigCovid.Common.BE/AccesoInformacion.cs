using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("AccesoInformacion")]
    public class AccesoInformacion
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public int EmpresaId { get; set; }
        public int SedeId { get; set; }
    }
}