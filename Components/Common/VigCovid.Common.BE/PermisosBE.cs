namespace VigCovid.Common.BE
{
    public class PermisosBE
    {
        public bool AsignarPacientes { get; set; }
        public bool PacientesAsignados { get; set; }
        public bool AccesoOtrosPacientesLectura { get; set; }
        public bool AccesoOtrosPacientesModificacion { get; set; }
    }
}