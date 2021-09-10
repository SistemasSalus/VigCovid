using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("HeadCount")]
    public class HeadCount
    {
        public int Id { get; set; }
        public string HC { get; set; }
        public string EmpresaEmpleadora { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string Dni { get; set; }
        public string Sexo { get; set; }
        public string CorreoTrabajador { get; set; }
        public string Sede { get; set; }
        public string Puesto { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string CorreoSupervisor { get; set; }
        public string HCM { get; set; }
        public string DivisionPersonal { get; set; }
        public string CentroCoste { get; set; }

        public string OrganizationId { get; set; }

        public string IDjefe { get; set; }

        public string PAEmployee { get; set; }


    }
}