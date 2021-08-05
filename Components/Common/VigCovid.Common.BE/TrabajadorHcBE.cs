using System;

namespace VigCovid.Common.BE
{
    public class TrabajadorHcBE
    {
        public string HC { get; set; }
        public string EmpresaEmpleadora { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string Dni { get; set; }
        public string CorreoTrabajador { get; set; }
        public string Sede { get; set; }
        public string Sexo { get; set; }
        public string Puesto { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string CorreoSupervisor { get; set; }

        public string OrganizationId { get; set; }

        public string NombreEmpresa { get; set; }

    }
}