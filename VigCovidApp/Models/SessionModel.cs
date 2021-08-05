using System.Collections.Generic;
using VigCovidApp.ViewModels;

namespace VigCovidApp.Models
{
    public class SessionModel
    {
        public int IdUser { get; set; }
        public int IdTipoUsuario { get; set; }
        public string UserName { get; set; }
        public bool AsignarPacientes { get; set; }
        public bool PacientesAsignados { get; set; }
        public bool AccesoOtrosPacientesLectura { get; set; }
        public bool AccesoOtrosPacientesModificacion { get; set; }

        public int EmpresaId { get; set; }

        public List<EmpresaSedeViewModel> EmpresasAsignadas { get; set; }
        
    }
}