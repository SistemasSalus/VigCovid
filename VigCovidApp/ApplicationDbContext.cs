namespace VigCovidApp
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using VigCovidApp.Models;

    public partial class ApplicationDbContext : DbContext
    {
        //public ApplicationDbContext()
        //    : base("name=ApplicationDbContext")
        //{
        //}

        //public virtual DbSet<RegistroTrabajador> RegistroTrabajador { get; set; }
        //public virtual DbSet<Seguimiento> Seguimiento { get; set; }
        //public virtual DbSet<Examen> Examen { get; set; }
        //public virtual DbSet<FechaImportante> FechaImportante { get; set; }

    }
}
