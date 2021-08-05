namespace VigCovid.Common.BE
{
    public class ListaTrabajadoresBE
    {
        public int RegistroTrabajadorId { get; set; }
        public string NombreCompleto { get; set; }
        public string ApellidosNombres_ { get; set; }
        public string NombreEmpresa_ { get; set; }
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
        public int MedicoVigilaId { get; set; }
        public string MedicoVigila { get; set; }
        public int UsuarioIngresa { get; set; }

        public int ModoIngresoId { get; set; }
        public int ViaIngresoId { get; set; }
        public string ViaIngreso { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string PuestoTrabajo { get; set; }
        public string Celular { get; set; }
        public string TelfReferencia { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Sexo { get; set; }
        public int EmpresaId { get; set; }
        public string NombreEmpresa { get; set; }
        public string Empleadora { get; set; }
        public int SedeId { get; set; }
        public string NombreSede { get; set; }
        public string ComentarioAlta { get; set; }
        public string NombreContacto { get; set; }
        public int? TipoContactoId { get; set; }
        public string TipoContacto { get; set; }
        public int Eliminado { get; set; }
        public string FechaIngresa { get; set; }
        public int? UsuarioActualiza { get; set; }
        public string FechaActualiza { get; set; }
        public string HCM { get; set; }
        public string DivisionPersonal { get; set; }
        public string CentroCoste { get; set; }
        public string ApellidosNombres { get; set; }

        public string FechaAlta { get; set; }

        public int TipoEmpresaId { get; set; }
    }
}