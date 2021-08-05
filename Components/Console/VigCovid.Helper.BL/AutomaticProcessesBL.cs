using System.Linq;
using VigCovid.Common.AccessData;

namespace VigCovid.Helper.BL
{
    public class AutomaticProcessesBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int CambiarAsigancionMedico()
        {
            var registros = (from A in db.RegistroTrabajador
                             where A.UsuarioIngresa == 41
                             select A).ToList();

            foreach (var item in registros)
            {
            }

            return 0;
        }
    }
}