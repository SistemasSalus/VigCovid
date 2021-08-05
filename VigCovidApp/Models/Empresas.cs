using System.Collections.Generic;
using System.Web.Mvc;
using static VigCovid.Common.Resource.Enums;

namespace VigCovidApp.Models
{
    public class Empresas
    {
        public Empresas()
        {
        }

        public List<SelectListItem> ListarEmpresas()
        {
            var listaEmpresas = new List<SelectListItem>();

            listaEmpresas.Add(new SelectListItem { Text = "SALUS LABORIS-CSO", Value = ((int)CodigoEmpresa.SalusLaboris).ToString() });
            listaEmpresas.Add(new SelectListItem { Text = "BACKUS", Value = ((int)CodigoEmpresa.Backus).ToString() });
            listaEmpresas.Add(new SelectListItem { Text = "AMBEV", Value = ((int)CodigoEmpresa.Ambev).ToString() });

            return listaEmpresas;
        }
    }
}