using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("Empresa")]
    public class Empresa
    {
        public int Id { get; set; }
        public string CodigoEmpresa { get; set; }
        public string NombreEmpresa { get; set; }

        public string OrganizationId { get; set; }

    }
}