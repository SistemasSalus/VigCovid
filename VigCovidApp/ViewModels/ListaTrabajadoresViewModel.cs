namespace VigCovidApp.ViewModels
{
    public class ListaTrabajadoresViewModel
    {
        public int Indice { get; set; }
        public int RegistroTrabajadorId { get; set; }
        public string NombreCompleto { get; set; }
        public string Dni { get; set; }
        public string Empresa { get; set; }
        public string Sede { get; set; }
        public int Edad { get; set; }
        public string FechaIngreso { get; set; }
        public string ModoIngreso { get; set; }
        public string HistorialEnfermedad { get; set; }
        public string EstadoDiario { get; set; }
        public string Clasificacion { get; set; }
        public string UltimoComentario { get; set; }
        public string UltimoSeguimiento { get; set; }
        public bool Alerta { get; set; }

        public string ContadorSeguimiento { get; set; }
        public string DiaSinSintomas { get; set; }

        public int? EstadoClinicoId { get; set; }

        public string MedicoVigila { get; set; }

        public bool Propietario { get; set; }
        public bool PermisoLectura { get; set; }
        public bool PermisoMoficacion { get; set; }
        public string FechaAlta { get; set; }
        public string EmpresaEmpleadora { get; set; }

        public int? EmpresaId { get; set; }

        public int? TipoEmpresaId { get; set; }



    }
}