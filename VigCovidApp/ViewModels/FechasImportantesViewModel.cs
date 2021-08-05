namespace VigCovidApp.ViewModels
{
    public class FechasImportantesViewModel
    {
        public int Id { get; set; }
        public int TrabajadorId { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }

        public bool? DM { get; set; }


    }
}