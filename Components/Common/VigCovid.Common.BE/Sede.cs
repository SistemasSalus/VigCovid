using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("Sede")]
    public class Sede
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string NombreSede { get; set; }
        public string Correos { get; set; }
        public string CorreosChampion { get; set; }
        public string CorreosBP { get; set; }
        public string CorreosPeople { get; set; }
        public string CorreosLideres { get; set; }
        public string CorreosSeguridadFisica { get; set; }
        public string CorreosSafety { get; set; }
        public string CorreosMedico { get; set; }
        public string CorreosCoordinador { get; set; }

        public string CorreoLicenciaCompensable { get; set; }

        public string CorreosTodaslasSedes { get; set; }

        public string CorreosSedesProvincia { get; set; }

        public string CorreosSedesLima { get; set; }

        public string MedicoEncargado { get; set; }

        

    }
}