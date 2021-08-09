namespace VigCovid.Common.BE
{
    public class RequestDarAltaBE
    {
        public int TrabajadorId { get; set; }
        public int SeguimientoId { get; set; }
        public string ComentarioAlta { get; set; }

        public string Receta { get; set; }

        public string FechaAlta { get; set; }
        public string Fecha { get; set; }

        public int UsuarioId { get; set; }

        public bool DM { get; set; }

        public string Descripcion { get; set; }

        public string FechaInicio { get; set; }

        public string FechaFin { get; set; }

        public int TipoRango { get; set; }

        public string Diagnostico { get; set; }

        public string EmpresaPrincipalId { get; set; }

    }
}