namespace VigCovidApp.ViewModels
{
    public class EmpresaSedeViewModel
    {
        public int UsuarioId { get; set; }
        public int TipoUsuarioId { get; set; }
        public string EmpresaIdSedeId { get; set; }
        public string EmpresaSede { get; set; }

        public int EmpresaId { get; set; } //Añadido por Saul 13052021

        //public List<EmpresaSede> Sedes { get; set; }
    }

    //public class EmpresaSede
    //{
    //    public int SedeId { get; set; }
    //    public string SedeNombre { get; set; }

    //}
}