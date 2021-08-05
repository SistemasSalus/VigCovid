using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VigCovid.Common.BE
{
    [Table("RegistroTrabajador")]
    public class RegistroTrabajador
    {
        [Key]
        public int Id { get; set; }

        public int ModoIngreso { get; set; }
        public int ViaIngreso { get; set; }

        
        public DateTime FechaIngreso { get; set; }
        public string NombreCompleto { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Dni { get; set; }
        public int Edad { get; set; }
        public string PuestoTrabajo { get; set; }
        public string Celular { get; set; }
        public string TelfReferencia { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Sexo { get; set; }
        public int EmpresaId { get; set; }
        public int SedeId { get; set; }
        public string Empleadora { get; set; }
        public int MedicoVigilaId { get; set; }
        public int? EstadoClinicoId { get; set; }
        public string ComentarioAlta { get; set; }

        public string Recetamedica { get; set; }

        public int? TipoEmpresaId { get; set; }

        [NotMapped]
        public DateTime? FechaExamen { get; set; }

        [NotMapped]
        public int? TipoExamen { get; set; }

        [NotMapped]
        public int? ResultadoExamen { get; set; }

        public string NombreContacto { get; set; }
        public int? TipoContacto { get; set; }
        public int Eliminado { get; set; }
        public int UsuarioIngresa { get; set; }
        public DateTime FechaIngresa { get; set; }
        public int UsuarioActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }

        [NotMapped]
        public bool? ConfirmarDuplicidadNombre { get; set; }

        
    }
}