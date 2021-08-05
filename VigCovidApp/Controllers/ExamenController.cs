using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;
using VigCovid.Security;
using VigCovidApp.Controllers.Base;
using VigCovidApp.Models;
using VigCovidApp.ViewModels;

namespace VigCovidApp.Controllers
{
    public class ExamenController : GenericController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public JsonResult CrearExamen(RegistrarExamen registrarExamen)
        {
            try
            {
                //ValidarPermisoModificacion
                if (!PermisosModificacion(registrarExamen.TrabajadorId)) return Json(false, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                var oExamen = new Examen();

                oExamen.Fecha = registrarExamen.FechaExamen;
                oExamen.TrabajadorId = registrarExamen.TrabajadorId;
                oExamen.TipoPrueba = registrarExamen.TipoExamen;
                oExamen.Resultado = registrarExamen.ResultadoExamen;

                db.Examen.Add(oExamen);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public void ActualizarExamen()
        {
        }

        public JsonResult ListarExamenesTrabajador(int trabajadorId)
        {
            var query = (from A in db.Examen where A.TrabajadorId == trabajadorId select A).ToList();

            var examenes = new List<ListarExamenesViewModel>();
            foreach (var item in query)
            {
                var examen = new ListarExamenesViewModel();
                examen.IdExamen = item.Id;
                examen.IdTrabajador = item.TrabajadorId;
                examen.FechaExamen = item.Fecha;
                   //examen.TipoPrueba = item.TipoPrueba == 0 ? "Prueba rápida" : "Molecular";
            //Modificado por Saul Ramos  - 09072021
                examen.TipoPrueba = item.TipoPrueba == 1 ? "Prueba rápida" : item.TipoPrueba == 2 ? "Molecular" : "Antígeno";
                examen.Resultado = ObtenerResultado(item.Resultado);

                examenes.Add(examen);
            }

            return Json(examenes, JsonRequestBehavior.AllowGet);
        }

        private string ObtenerResultado(int resultado)
        {
            if (resultado == 0)
            {
                return "Negativo";
            }
            else if (resultado == 1)
            {
                return "No válido";
            }
            else if (resultado == 2)
            {
                return "IgM Positivo";
            }
            else if (resultado == 3)
            {
                return "IgG Positivo";
            }
            else if (resultado == 4)
            {
                return "IgM e IgG Positivo";
            }
            else if (resultado == 6)
            {
                return "Positivo";
            }

            else
            {
                return "";
            }
        }

        private bool PermisosModificacion(int trabajadorId)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            //Es propietario
            var esPropietario = new AccessBL().EsPropietario(sessione.IdUser, trabajadorId);

            if (!esPropietario)
            {
                if (!sessione.AccesoOtrosPacientesModificacion)
                {
                    return false;
                }
            }

            return true;
        }
    }
}