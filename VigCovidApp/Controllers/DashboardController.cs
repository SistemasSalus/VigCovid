using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VigCovid.Dashboard.BL;
using VigCovidApp.Controllers.Base;
using VigCovidApp.Models;
using VigCovidApp.ViewModels;

namespace VigCovidApp.Controllers
{
    public class DashboardController : GenericController
    {
        public JsonResult GraficoCasosDiarios(string sedesId)
        {
            var oDashboardBL = new DashboardBL();
            var result = new List<CasosDiariosViewModel>();
            var ff = DateTime.Now;
            var fi = DateTime.Now.AddDays(-17);

            var lista = oDashboardBL.CasosDiarios(fi, ff, sedesArr(sedesId));

            foreach (var item in lista)
            {
                var oCasosDiariosViewModel = new CasosDiariosViewModel();
                oCasosDiariosViewModel.dia = item.dia;

                #region Asintomáticos

                oCasosDiariosViewModel.CasosDiariosAsintomaticos = item.CasosDiariosAsintomaticos;

                #endregion Asintomáticos

                #region Sospechosos

                oCasosDiariosViewModel.CasosDiariosSospechosos = item.CasosDiariosSospechosos;

                #endregion Sospechosos

                #region Confirmado Sintomatico

                oCasosDiariosViewModel.CasosDiariosConfirmadoSintomatico = item.CasosDiariosConfirmadoSintomatico;

                #endregion Confirmado Sintomatico

                result.Add(oCasosDiariosViewModel);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GraficoAltas(string sedesId)
        {
            var oDashboardBL = new DashboardBL();
            var result = new List<CasosAltasViewModel>();
            var fi = DateTime.Now.AddDays(-17);
            var ff = DateTime.Now.AddDays(365);

            var oAltas = oDashboardBL.Altas(fi, ff, sedesArr(sedesId));

            foreach (var item in oAltas)
            {
                var oCasosAltasViewModel = new CasosAltasViewModel();
                oCasosAltasViewModel.Dia = item.Dia;
                oCasosAltasViewModel.Tipo = item.Tipo;
                oCasosAltasViewModel.Numero = item.Numero;
                result.Add(oCasosAltasViewModel);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GraficoSedes(string sedesId, string Mes)
        {
            var oDashboardBL = new DashboardBL();
            var result = new List<CasosSedesViewModel>();

            var ff = DateTime.Now;
            var fi = DateTime.Now;

            if (Mes == "MARZO")
            {
                fi = DateTime.Parse("01-03-2020");
                ff = DateTime.Parse("01-04-2020");
            }
            else if (Mes == "ABRIL")
            {
                fi = DateTime.Parse("01-04-2020");
                ff = DateTime.Parse("01-05-2020");
            }
            else if (Mes == "MAYO")
            {
                fi = DateTime.Parse("01-05-2020");
                ff = DateTime.Parse("01-06-2020");
            }
            else if (Mes == "JUNIO")
            {
                fi = DateTime.Parse("01-06-2020");
                ff = DateTime.Parse("01-07-2020");
            }
            else if (Mes == "JULIO")
            {
                fi = DateTime.Parse("01-07-2020");
                ff = DateTime.Parse("01-08-2020");
            }
            else if (Mes == "AGOSTO")
            {
                fi = DateTime.Parse("01-08-2020");
                ff = DateTime.Parse("01-09-2020");
            }
            else if (Mes == "SETIEMBRE")
            {
                fi = DateTime.Parse("01-09-2020");
                ff = DateTime.Parse("01-10-2020");
            }
            else if (Mes == "OCTUBRE")
            {
                fi = DateTime.Parse("01-10-2020");
                ff = DateTime.Parse("01-11-2020");
            }
            else if (Mes == "NOVIEMBRE")
            {
                fi = DateTime.Parse("01-11-2020");
                ff = DateTime.Parse("01-12-2020");
            }
            else if (Mes == "DICIEMBRE")
            {
                fi = DateTime.Parse("01-12-2020");
                ff = DateTime.Parse("01-01-2021");
            }

            var lista = oDashboardBL.Sedes(fi, ff, sedesArr(sedesId));

            foreach (var item in lista)
            {
                var oCasosSedesViewModel = new CasosSedesViewModel();
                oCasosSedesViewModel.Sede = item.Sede;

                #region Asintomáticos

                oCasosSedesViewModel.CasosAsintomaticos = item.CasosAsintomaticos;

                #endregion Asintomáticos

                #region Sospechosos

                oCasosSedesViewModel.CasosSospechosos = item.CasosSospechosos;

                #endregion Sospechosos

                #region Confirmado Sintomatico

                oCasosSedesViewModel.CasosConfirmadoSintomatico = item.CasosConfirmadoSintomatico;

                #endregion Confirmado Sintomatico

                result.Add(oCasosSedesViewModel);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<int> sedesArr(string sedesId)
        {
            if (string.IsNullOrEmpty(sedesId))
            {
                var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
                var empresasAsignadas = sessione.EmpresasAsignadas;

                foreach (var item in empresasAsignadas)
                {
                    var sedeId = item.EmpresaIdSedeId.Split('-')[1];
                    sedesId += sedeId + ",";
                }
                sedesId = sedesId.Substring(0, sedesId.Length - 1);
            }

            var arrCodigos = sedesId.Split(',');
            var arrint = Array.ConvertAll(arrCodigos, int.Parse).ToList();

            return arrint;
        }
    }
}