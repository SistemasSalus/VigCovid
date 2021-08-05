using System.Collections.Generic;
using System.Linq;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;

namespace VigCovid.Worker.BL
{
    public class EmpresaBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Empresa> GetAllEmpresas()
        {
            var empresas = (from A in db.Empresa select A).ToList();

            return empresas;
        }
    }
}