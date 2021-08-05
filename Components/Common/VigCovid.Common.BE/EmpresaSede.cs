using System.Collections.Generic;

namespace VigCovid.Common.BE
{
    public class EmpresaBE
    {
        public int EmpresaId { get; set; }
        public string EmpresaNombre { get; set; }
        public string OrganizationId { get; set; }
        public List<SedeBE> Sedes { get; set; }
    }

    public class SedeBE
    {
        public int SedeId { get; set; }
        public string SedeNombre { get; set; }
    }
}