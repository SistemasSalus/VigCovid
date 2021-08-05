using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;

namespace VigCovid.Worker.BL
{
    public class EmpresaSedeBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<SedeBE> GetSedeEmpresaPrincipal(int empresaId)
        {
            //var oSedeBE = new SedeBE();
            var list = (from A in db.Sede
                        where A.EmpresaId == empresaId
                        select new SedeBE
                        {
                            SedeId = A.Id,
                            SedeNombre = A.NombreSede
                        }).ToList();

            return list;
        }
    }
}
