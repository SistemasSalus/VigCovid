using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public int PersonaId { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordUsuario { get; set; }
        public int TipoUsuarioId { get; set; }
    }
}