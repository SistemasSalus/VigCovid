using System.Data.Entity;
using VigCovid.Common.BE;

namespace VigCovid.Common.AccessData
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("name=ApplicationDbContext")
        {
        }

        public virtual DbSet<Parametro> Parametro { get; set; }
        public virtual DbSet<RegistroTrabajador> RegistroTrabajador { get; set; }
        public virtual DbSet<Seguimiento> Seguimiento { get; set; }
        public virtual DbSet<Examen> Examen { get; set; }
        public virtual DbSet<FechaImportante> FechaImportante { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<AccesoInformacion> AccesoInformacion { get; set; }
        public virtual DbSet<Empresa> Empresa { get; set; }
        public virtual DbSet<Sede> Sede { get; set; }
        public virtual DbSet<HeadCount> HeadCount { get; set; }
        public virtual DbSet<Persona> Persona { get; set; }
    }
}