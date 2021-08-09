using iTextSharp.text;
using iTextSharp.text.pdf;
using NetPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;
using VigCovid.Common.Resource;
using VigCovid.MedicalMonitoring.BL;
using VigCovid.Report.BL;
using VigCovid.Security;
using VigCovidApp.Controllers.Base;
using VigCovidApp.Models;
using VigCovidApp.Utils;
using VigCovidApp.ViewModels;

using static VigCovid.Common.Resource.Enums;

namespace VigCovidApp.Controllers
{
    public class MonitoringWorkerController : GenericController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult RegisterMonitoring(int id)
        {
            //ViewBag.COMBOCALIFICACION = ComboCalificacion();
            ViewBag.REGISTROTRABAJADOR = id;

            ViewBag.CABECERA = GetCabecera(id);

            ViewBag.FECHASIMPORTANTES = GetFechasImportantes(id);

            ViewBag.TIPOSDERANGO = GetTipoRango(id);

            ViewBag.EXAMENES = ListarExamenesTrabajador(id);

            var seguimientos = Seguimientos(id);
            if (seguimientos.Count() == 0) return View(new List<SeguimientosViewModel>());

            return View(seguimientos);
        }

        public ActionResult RemoveTab(int SeguimientoId, int RegistroTrabajadorId)
        {
            var seguimiento = db.Seguimiento.Where(x => x.Id == SeguimientoId).FirstOrDefault();
            if (seguimiento != null)
            {
                if (seguimiento.TipoEstadoId == -1)
                {
                    seguimiento.Eliminado = 1;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("RegisterMonitoring", "MonitoringWorker", new { @id = RegistroTrabajadorId });
        }



        public JsonResult GetFechasImportantesJson(int trabajadorId)
        {
            var oLineaTiempoBL = new LineaTiempoBL();
            var result = oLineaTiempoBL.GetAllFechasLineaTiempo(trabajadorId);

            return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public List<LineaTiempo> GetFechasImportantes(int trabajadorId)
        {
            var oLineaTiempoBL = new LineaTiempoBL();
            return oLineaTiempoBL.GetAllFechasLineaTiempo(trabajadorId);

           
        }


        public JsonResult GetTipoRangoJson(int trabajadorId)
        {
            var oLineaTiempoBL = new LineaTiempoBL();
            var result = oLineaTiempoBL.GetAllTipoRango(trabajadorId);

            return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }


        public List<LineaTiempo> GetTipoRango(int trabajadorId)
        {
            var oLineaTiempoBL = new LineaTiempoBL();
            return oLineaTiempoBL.GetAllTipoRango(trabajadorId);
        }



        [HttpGet]
        public ActionResult NewRegisterMonitoring(int id)
        {
            try
            {
                var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

                #region Obtener Nro Seguimiento siguiente

                var nroSeguimientoSiguiente = GetNroMonitoring(id) + 1;

                #endregion Obtener Nro Seguimiento siguiente

                #region Validar que se creé un seguimiento al día

                if (ValidateMonitoring(id) != null) return null;

                #endregion Validar que se creé un seguimiento al día

                #region Grabar Nuevo Seguimiento

                var oSeguimiento = new Seguimiento();
                oSeguimiento.RegistroTrabajadorId = id;
                oSeguimiento.Fecha = DateTime.Now.Date;
                oSeguimiento.ClasificacionId = -1;

                oSeguimiento.SensacionFiebre = false;
                oSeguimiento.Tos = false;
                oSeguimiento.DolorGarganta = false;
                oSeguimiento.DificultadRespiratoria = false;

                oSeguimiento.CongestionNasal = false;
                oSeguimiento.Cefalea = false;
                oSeguimiento.MalestarGeneral = false;
                oSeguimiento.PerdidaOlfato = false;
                oSeguimiento.Asintomatico = false;

                oSeguimiento.Comentario = "";
                oSeguimiento.Recetamedica = "";
                //oSeguimiento.ResultadoCovid19 = 1;
                oSeguimiento.TipoEstadoId = -1;
                oSeguimiento.NroSeguimiento = nroSeguimientoSiguiente;

                oSeguimiento.Eliminado = 0;
                oSeguimiento.UsuarioIngresa = sessione.IdUser;
                oSeguimiento.FechaIngresa = DateTime.Now;

                db.Seguimiento.Add(oSeguimiento);
                db.SaveChanges();

                #endregion Grabar Nuevo Seguimiento

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult ListarFechasImportabtes(int idTrabajador)
        {
            try
            {
                var oFechaImportanteBL = new FechaImportanteBL();
                var result = oFechaImportanteBL.GetAllFechasImportantes(idTrabajador);

                var listaFechas = new List<FechasImportantesViewModel>();
                foreach (var item in result)
                {
                    var oFechasImportantesViewModel = new FechasImportantesViewModel();
                    oFechasImportantesViewModel.Id = item.Id;
                    oFechasImportantesViewModel.TrabajadorId = item.TrabajadorId;
                    oFechasImportantesViewModel.Descripcion = item.Descripcion;
                    oFechasImportantesViewModel.Fecha = item.Fecha.Value.ToString("dd/MM/yyyy");
                    oFechasImportantesViewModel.DM = item.DM;

                    listaFechas.Add(oFechasImportantesViewModel);
                }

                return Json(listaFechas, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public JsonResult ListarTiposdeRango(int trabajadorId)
        {
            var fechasImportantes = (from A in db.FechaImportante where A.TrabajadorId == trabajadorId && A.TipoRango >= 1 select A).ToList();
            var fechas = new List<LineaTiempo>();

            foreach (var item in fechasImportantes)
            {
                var oFechaInicioSintomas = new LineaTiempo();
                oFechaInicioSintomas.FechaInicio = item.FechaInicio.Value;
                oFechaInicioSintomas.FechaFin = item.FechaFin.Value;
                oFechaInicioSintomas.Descripcion = item.Descripcion;
                fechas.Add(oFechaInicioSintomas);



            }

            return Json(fechas, JsonRequestBehavior.AllowGet);
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

        private bool PacienteConAlta(int trabajadorId)
        {
            var registroTrabajador = (from A in db.RegistroTrabajador where A.Id == trabajadorId select A).FirstOrDefault();
            if (registroTrabajador.EstadoClinicoId == null)
                return false;

            if (registroTrabajador.EstadoClinicoId.Value == (int)Enums.EstadoClinico.alta)
                return true;

            return false;
        }

        public JsonResult ValidarAntesSalir(int trabajadorId)
        {
            var oFechaImportanteBL = new FechaImportanteBL();
            var result = oFechaImportanteBL.ValidarFechasSeguimiento(trabajadorId);

            return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSeguimiento(ActualizarSeguimientoViewModel entidad)
        {
            try
            {
                var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

                //ValidarPermisoModificacion
                if (!PermisosModificacion(entidad.RegistroTrabajadorId)) return Json(false, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                //Paciente con Alta
                if (PacienteConAlta(entidad.RegistroTrabajadorId)) return Json(false, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                var result = db.Seguimiento.SingleOrDefault(b => b.Id == entidad.Id);
                if (result != null)
                {
                    //result.ClasificacionId  = entidad.ClasificacionId;
                    result.SensacionFiebre = entidad.SensacionFiebre;
                    result.Tos = entidad.Tos;
                    result.DolorGarganta = entidad.DolorGarganta;
                    result.DificultadRespiratoria = entidad.DificultadRespiratoria;

                    result.CongestionNasal = entidad.CongestionNasal;
                    result.Cefalea = entidad.Cefalea;
                    result.MalestarGeneral = entidad.MalestarGeneral;
                    result.PerdidaOlfato = entidad.PerdidaOlfato;
                    result.Asintomatico = entidad.Asintomatico;
                    //--------------------------------------------------
                    result.HipertensionArterial = entidad.HipertensionArterial;
                    result.HipertensionArterialNoControlada = entidad.HipertensionArterialNoControlada;
                    result.AsmaModeradoSevero = entidad.AsmaModeradoSevero;
                    result.Diabetes = entidad.Diabetes;
                    result.Mayor65 = entidad.Mayor65;
                    result.Cancer = entidad.Cancer;
                    result.CardiovascularGrave = entidad.CardiovascularGrave;
                    result.ImcMayor40 = entidad.ImcMayor40;
                    result.RenalDialisis = entidad.RenalDialisis;
                    result.PulmonarCronica = entidad.PulmonarCronica;
                    result.TratInmunosupresor = entidad.TratInmunosupresor;

                    result.CasoPositivo = entidad.CasoPositivo;
                    result.CasoSospechoso = entidad.CasoSospechoso;
                    result.RinofaringitisAguda = entidad.RinofaringitisAguda;
                    result.NeumoniaViral = entidad.NeumoniaViral;
                    result.ContactoEnfermedades = entidad.ContactoEnfermedades;
                    result.Aislamiento = entidad.Aislamiento;
                    result.Otros = entidad.Otros;
                    result.OtrosComentar = entidad.OtrosComentar;

                    result.Comentario = entidad.Comentario;
                    result.Recetamedica = entidad.Recetamedica;
                    //result.ResultadoCovid19 = entidad.ResultadoCovid19;
                    //result.FechaResultadoCovid19 = entidad.FechaResultadoCovid19;
                    result.ProximoSeguimiento = entidad.ProximoSeguimiento;
                    result.TipoEstadoId = entidad.TipoEstadoId;
                    result.PAntigeno = entidad.PAntigeno;
                    result.PAntigenos = entidad.PAntigenos;
                    result.PAntigenos5 = entidad.PAntigenos5;
                    result.PulsoOximetro = entidad.PulsoOximetro;
                    result.Eliminado = 0;
                    result.UsuarioActualiza = sessione.IdUser;
                    result.FechaActualiza = DateTime.Now;

                    db.SaveChanges();
                }

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateFechaSeguimiento(FechaSeguimiento entidad)
        {
            try
            {
                #region Validar que se creé un seguimiento al día

                if (ValidateMonitoring(entidad.TrabajadorId, entidad.Fecha) != null) return null;

                #endregion Validar que se creé un seguimiento al día

                var result = db.Seguimiento.SingleOrDefault(b => b.Id == entidad.SeguimientoId);
                if (result != null)
                {
                    result.Fecha = entidad.Fecha;
                    db.SaveChanges();
                }

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult UpdateEstadoClinico(EstadoClinico oEstadoClinico)
        {
            try
            {
                var result = db.RegistroTrabajador.SingleOrDefault(b => b.Id == oEstadoClinico.TrabajadorId);
                if (result != null)
                {
                    result.EstadoClinicoId = (int)Enums.EstadoClinico.alta;
                    db.SaveChanges();
                }

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult ComentarioAlta(ComentarioAltaViewModel oComentarioAlta)
        {
            try
            {
                var result = db.RegistroTrabajador.SingleOrDefault(b => b.Id == oComentarioAlta.TrabajadorId);
                if (result != null)
                {
                    result.ComentarioAlta = oComentarioAlta.Comentario;
                    db.SaveChanges();

                    GenerarAltaMedica(result.Id, oComentarioAlta.Comentario);
                }

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private List<SelectListItem> ComboCalificacion()
        {
            List<SelectListItem> opcionesCalificacion = new List<SelectListItem>();
            opcionesCalificacion.Add(new SelectListItem { Text = " ", Value = "-1" });
            opcionesCalificacion.Add(new SelectListItem { Text = "Asintomático", Value = "1" });
            opcionesCalificacion.Add(new SelectListItem { Text = "Sintomático Leve", Value = "2" });
            opcionesCalificacion.Add(new SelectListItem { Text = "Sintomático Moderado", Value = "3" });

            return opcionesCalificacion;
        }

        [HttpPost]
        public JsonResult SaveFechaImportante(FechaImportante entidad)
        {
            try
            {
                //ValidarPermisoModificacion
                if (!PermisosModificacion(entidad.TrabajadorId)) return Json(false, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

                var oFechaImportanteBL = new FechaImportanteBL();
                var result = oFechaImportanteBL.SaveFechaImportante(entidad);

                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult DarAltaTrabajador(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().DarAlta(oRequestDarAltaBE);

                if (result == false)
                {
                    GenerarAltaMedica(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.ComentarioAlta);
                }
                else
                {
                    GenerarAltaMedica(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.ComentarioAlta);
                }
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }






        [HttpPost]
        public void EnviarSolicituddePruebas(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];



            SolicituddePruebasEmail(oRequestDarAltaBE.TrabajadorId);

        }



        [HttpPost]
        public void EnviarSolicituddePruebas5dias(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];



            SolicituddePruebasEmail5(oRequestDarAltaBE.TrabajadorId);

        }

        [HttpPost]
        public void EnviarSolicituddePruebasPulsoOximetro(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];



            SolicituddePruebasEmailPulsoOximetro(oRequestDarAltaBE.TrabajadorId);

        }



        [HttpPost]
        public JsonResult EnviarDescansoMedico1(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().DescansosMedicosActualizar(oRequestDarAltaBE);

                

                if (oRequestDarAltaBE.TipoRango == 1)
                {
                    EnviarDM(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.EmpresaPrincipalId, oRequestDarAltaBE.Diagnostico);
                }
                else
                {
                
                }

               // GetTipoRango(oRequestDarAltaBE.TrabajadorId);
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
                return Json(result, JsonRequestBehavior.AllowGet);
               // return Json(result,"aplication/json",Encoding.UTF8);



            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult EnviarDescansoMedico2(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().DescansosMedicosActualizar(oRequestDarAltaBE);

                if (result == false)
                {
                    EnviarDM2(oRequestDarAltaBE.TrabajadorId);
                }
                else
                {
                    EnviarDM2(oRequestDarAltaBE.TrabajadorId);
                }
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult EnviarDescansoMedico3(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().DescansosMedicosActualizar(oRequestDarAltaBE);

                if (result == false)
                {
                    EnviarDM3(oRequestDarAltaBE.TrabajadorId);
                }
                else
                {
                    EnviarDM3(oRequestDarAltaBE.TrabajadorId);
                }
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult EnviarDescansoMedico4(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().DescansosMedicosActualizar(oRequestDarAltaBE);

                if (result == false)
                {
                    EnviarDM4(oRequestDarAltaBE.TrabajadorId);
                }
                else
                {
                    EnviarDM4(oRequestDarAltaBE.TrabajadorId);
                }
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult EnviarDescansoMedico5(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().DescansosMedicosActualizar(oRequestDarAltaBE);

                if (result == false)
                {
                    EnviarDM5(oRequestDarAltaBE.TrabajadorId);
                }
                else
                {
                    EnviarDM5(oRequestDarAltaBE.TrabajadorId);
                }
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult EnviarDescansoMedico6(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().DescansosMedicosActualizar(oRequestDarAltaBE);

                if (result == false)
                {
                    EnviarDM6(oRequestDarAltaBE.TrabajadorId);
                }
                else
                {
                    EnviarDM6(oRequestDarAltaBE.TrabajadorId);
                }
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [HttpPost]
        public JsonResult DarRegistrarReceta(RequestDarAltaBE oRequestDarAltaBE)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

            try
            {
                oRequestDarAltaBE.UsuarioId = sessione.IdUser;
                var result = new DatosCabeceraBL().RecetasMedicasActualizar(oRequestDarAltaBE);

                if (result == false)
                {
                    RecetasMedicas(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.Receta);
                }
                else
                {
                    RecetasMedicas(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.Receta);
                }
                return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }





        #region Private Methodos

        private IEnumerable<SeguimientosViewModel> Seguimientos(int registroTrabajadorId)
        {
            try
            {
                var listaSeguimientos = new List<SeguimientosViewModel>();
                var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).ToList().OrderByDescending(p => p.NroSeguimiento);

                foreach (var seguimiento in seguimientos)
                {
                    var oSeguimiento = new SeguimientosViewModel();
                    oSeguimiento.SeguimientoId = seguimiento.Id;
                    oSeguimiento.RegistroTrabajadorId = seguimiento.RegistroTrabajadorId;
                    oSeguimiento.Fecha = seguimiento.Fecha.Date.ToString("dd-MMMM-yyy");
                    oSeguimiento.ClasificacionId = seguimiento.ClasificacionId;
                    oSeguimiento.SensacionFiebre = seguimiento.SensacionFiebre == true ? "checked" : "";
                    oSeguimiento.Tos = seguimiento.Tos == true ? "checked" : "";
                    oSeguimiento.DolorGarganta = seguimiento.DolorGarganta == true ? "checked" : "";
                    oSeguimiento.DificultadRespiratoria = seguimiento.DificultadRespiratoria == true ? "checked" : "";

                    oSeguimiento.CongestionNasal = seguimiento.CongestionNasal == true ? "checked" : "";
                    oSeguimiento.Cefalea = seguimiento.Cefalea == true ? "checked" : "";
                    oSeguimiento.MalestarGeneral = seguimiento.MalestarGeneral == true ? "checked" : "";
                    oSeguimiento.PerdidaOlfato = seguimiento.PerdidaOlfato == true ? "checked" : "";
                    oSeguimiento.Asintomatico = seguimiento.Asintomatico == true ? "checked" : "";
                    oSeguimiento.PAntigeno = seguimiento.PAntigeno == true ? "checked" : "";
                    oSeguimiento.PAntigenos = seguimiento.PAntigenos == true ? "checked" : "";
                    oSeguimiento.PAntigenos5 = seguimiento.PAntigenos5 == true ? "checked" : "";
                    oSeguimiento.PulsoOximetro = seguimiento.PulsoOximetro == true ? "checked" : "";


                    oSeguimiento.Comentario = seguimiento.Comentario;
                    oSeguimiento.Recetamedica = seguimiento.Recetamedica;
                    if (seguimiento.ProximoSeguimiento == null)
                    {
                        oSeguimiento.ProximoSeguimiento = "";
                    }
                    else
                    {
                        oSeguimiento.ProximoSeguimiento = seguimiento.ProximoSeguimiento.Value.ToString("dd/MM/yyyy");
                    }
                    oSeguimiento.TipoEstadoId = seguimiento.TipoEstadoId.Value;
                    oSeguimiento.NroSeguimiento = seguimiento.NroSeguimiento;
                    //-----------------------------------------------------
                    oSeguimiento.HipertensionArterial = seguimiento.HipertensionArterial == true ? "checked" : "";
                    oSeguimiento.HipertensionArterialNoControlada = seguimiento.HipertensionArterialNoControlada == true ? "checked" : "";
                    oSeguimiento.AsmaModeradoSevero = seguimiento.AsmaModeradoSevero == true ? "checked" : "";
                    oSeguimiento.Diabetes = seguimiento.Diabetes == true ? "checked" : "";
                    oSeguimiento.Mayor65 = seguimiento.Mayor65 == true ? "checked" : "";
                    oSeguimiento.Cancer = seguimiento.Cancer == true ? "checked" : "";
                    oSeguimiento.CardiovascularGrave = seguimiento.CardiovascularGrave == true ? "checked" : "";
                    oSeguimiento.ImcMayor40 = seguimiento.ImcMayor40 == true ? "checked" : "";
                    oSeguimiento.RenalDialisis = seguimiento.RenalDialisis == true ? "checked" : "";
                    oSeguimiento.PulmonarCronica = seguimiento.PulmonarCronica == true ? "checked" : "";
                    oSeguimiento.TratInmunosupresor = seguimiento.TratInmunosupresor == true ? "checked" : "";

                    oSeguimiento.CasoPositivo = seguimiento.CasoPositivo == true ? "checked" : "";
                    oSeguimiento.CasoSospechoso = seguimiento.CasoSospechoso == true ? "checked" : "";
                    oSeguimiento.RinofaringitisAguda = seguimiento.RinofaringitisAguda == true ? "checked" : "";
                    oSeguimiento.NeumoniaViral = seguimiento.NeumoniaViral == true ? "checked" : "";
                    oSeguimiento.ContactoEnfermedades = seguimiento.ContactoEnfermedades == true ? "checked" : "";
                    oSeguimiento.Aislamiento = seguimiento.Aislamiento == true ? "checked" : "";
                    oSeguimiento.Otros = seguimiento.Otros == true ? "checked" : "";
                    oSeguimiento.OtrosComentar = seguimiento.OtrosComentar;
                    listaSeguimientos.Add(oSeguimiento);
                }

                return listaSeguimientos;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private int GetNroMonitoring(int registroTrabajadorId)
        {
            var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).OrderByDescending(o => o.NroSeguimiento).ToList();
            if (seguimientos.Count == 0)
                return 0;

            return seguimientos[0].NroSeguimiento;
        }

        private Seguimiento ValidateMonitoring(int registroTrabajadorId)
        {
            var fechaHoy = DateTime.Now.Date;
            var result = db.Seguimiento.Where(p => p.RegistroTrabajadorId == registroTrabajadorId && p.Fecha == fechaHoy).FirstOrDefault();

            return result;
        }

        private Seguimiento ValidateMonitoring(int registroTrabajadorId, DateTime fecha)
        {
            var result = db.Seguimiento.Where(p => p.RegistroTrabajadorId == registroTrabajadorId && p.Fecha == fecha).FirstOrDefault();

            return result;
        }

        private CabeceraViewModel GetCabecera(int registroTrabajadorId)
        {
            var result = new CabeceraViewModel();

            var oDatosCabecera = new DatosCabeceraBL().GetCabecera(registroTrabajadorId);
            result.Trabajador = oDatosCabecera.Trabajador;
            result.Empresa = oDatosCabecera.Empresa;
            result.Celular = oDatosCabecera.Celular;
            result.Email = oDatosCabecera.Email;
            result.Sede = oDatosCabecera.Sede;
            result.Puesto = oDatosCabecera.Puesto;
            result.ModoIngreso = oDatosCabecera.ModoIngreso;
            result.ViaIngreso = oDatosCabecera.ViaIngreso;
            result.EstadoClinico = oDatosCabecera.EstadoClinico;
            return result;
        }

        private string CalcularDiasSeguimiento(List<Seguimiento> seguimientos)
        {
            var primerSeguimiento = seguimientos.Find(p => p.NroSeguimiento == 1).Fecha;

            var dias = (DateTime.Now - primerSeguimiento).TotalDays;
            var rDias = Decimal.Parse(dias.ToString());

            return Decimal.Round(rDias).ToString();
        }

        private IndicadorCovid19 GetDiasPrueba(List<Seguimiento> seguimientos)
        {
            var oIndicadorCovid19 = new IndicadorCovid19();
            var count = 0;
            var resultado = "";

            var sortSeguimientos = seguimientos.OrderByDescending(p => p.NroSeguimiento);

            foreach (var item in sortSeguimientos)
            {
                //if (!(item.FechaResultadoCovid19 == null || item.FechaResultadoCovid19 < DateTime.Now.AddYears(-5))
                //&& (!(item.ResultadoCovid19 == null || item.ResultadoCovid19 == -1)))
                //{
                //    resultado = GetResultadoCovid19(item.ResultadoCovid19.Value);
                //    break;
                //}
                //else
                //{
                //    count++;
                //}
            }
            oIndicadorCovid19.Contador = count;
            oIndicadorCovid19.Resultado = resultado;
            return oIndicadorCovid19;
        }

        private int GetNroDiasSinSintomas(List<Seguimiento> seguimientos)
        {
            var count = 0;

            foreach (var item in seguimientos)
            {
                if (item.SensacionFiebre == false
                    && item.Tos == false
                    && item.DolorGarganta == false
                    && item.DificultadRespiratoria == false
                    && item.CongestionNasal == false
                    && item.Cefalea == false
                    && item.MalestarGeneral == false
                    && item.PerdidaOlfato == false)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        private string GetFechaInicioSintomas(List<Seguimiento> seguimientos)
        {
            var sortSeguimientos = seguimientos.OrderBy(p => p.NroSeguimiento);
            var fechaInicioSintomas = "";
            foreach (var item in sortSeguimientos)
            {
                if (item.SensacionFiebre == true
                    || item.Tos == true
                    || item.DolorGarganta == true
                    || item.DificultadRespiratoria == true
                    || item.CongestionNasal == true
                    || item.Cefalea == true
                    || item.MalestarGeneral == true
                    || item.PerdidaOlfato == true)
                {
                    fechaInicioSintomas = item.Fecha.ToString("dd/MM/yyyy");
                    break;
                }
            }

            return fechaInicioSintomas;
        }

        private string GetEmpresa(int id)
        {
            if (id == 1)
            {
                return "SALUS LABORIS";
            }
            else if (id == 2)
            {
                return "BACKUS";
            }
            else if (id == 3)
            {
                return "AMBEV";
            }
            else
            {
                return "";
            }
        }

        private string GetResultadoCovid19(int id)
        {
            if (id == (int)ResultadoCovid19.Negativo)
            {
                return "Negativo";
            }
            else if (id == (int)ResultadoCovid19.Novalido)
            {
                return "No válido";
            }
            else if (id == (int)ResultadoCovid19.IgMPositivo)
            {
                return "IgM Positivo";
            }
            else if (id == (int)ResultadoCovid19.IgGPositivo)
            {
                return "IgG Positivo";
            }
            else if (id == (int)ResultadoCovid19.IgMeIgGpositivo)
            {
                return "IgM e IgG positivo";
            }
            else if (id == (int)ResultadoCovid19.Noserealizo)
            {
                return "No se realizó";
            }
            else
            {
                return "";
            }
        }

        private List<ListarExamenesViewModel> ListarExamenesTrabajador(int trabajadorId)
        {
            var query = (from A in db.Examen where A.TrabajadorId == trabajadorId select A).ToList();

            var examenes = new List<ListarExamenesViewModel>();
            foreach (var item in query)
            {
                var examen = new ListarExamenesViewModel();
                examen.IdExamen = item.Id;
                examen.IdTrabajador = item.TrabajadorId;
                examen.FechaExamen = item.Fecha;
                //examen.TipoPrueba = item.TipoPrueba == (int)TipoExamenCovid.Pr ? "Prueba rápida" : item.TipoPrueba == (int)TipoExamenCovid.Molecular ? "Molecular" : "Antígenos";
                examen.TipoPrueba = item.TipoPrueba == (int)TipoExamenCovid.Pr ? "Prueba rápida" : item.TipoPrueba == (int)TipoExamenCovid.Molecular ? "Molecular" : "Antígenos";


                examen.Resultado = ObtenerResultado(item.Resultado);

                examenes.Add(examen);
            }

            return examenes;
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

        #endregion Private Methodos

        #region Class

        private class IndicadorCovid19
        {
            public int Contador { get; set; }
            public string Resultado { get; set; }
        }

        public class FechaSeguimiento
        {
            public int TrabajadorId { get; set; }
            public int SeguimientoId { get; set; }
            public DateTime Fecha { get; set; }
        }

        public class EstadoClinico
        {
            public int TrabajadorId { get; set; }
        }

        #endregion Class




        public async Task<FileResult> ExportAltaMedica(int id, string comentario)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.ObtenerDatosAlta(id, sessione.IdUser);
            if (comentario != null)
            {
                datosAlta.ComentarioAlta = comentario;
            }

            MemoryStream memoryStream = GetPdfAltaMedica(datosAlta);

            // Hacer una copia en memoria, una sera usada para el correo y el proceso de devoler el PDF - Saul Ramos Vega
            var position = memoryStream.Position;
            var archivoPDFDescarga = new MemoryStream();
            memoryStream.CopyTo(archivoPDFDescarga);
            memoryStream.Position = position;
            archivoPDFDescarga.Position = position;

            if (comentario == null)
            {
                #region Envio correos

                var configEmail = new ReportAltaBL().ParametroCorreo();
                string smtp = configEmail[0].v_Value1.ToLower();
                int port = int.Parse(configEmail[1].v_Value1);
                string from = configEmail[2].v_Value1.ToLower();
                string fromPassword = configEmail[4].v_Value1;
                if (datosAlta.CorreosTrabajador == null)
                {
                    datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
                }
                if (datosAlta.CorreosChampios == null)
                {
                    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
                }

                //using (var mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios))
                using (var mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador))
                {
                    mailMessage.Subject = "FICHA DE ALTA COVID 19";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = "Se adjunta su ficha de alta";

                    mailMessage.Attachments.Add(new Attachment(memoryStream, "ficha de alta.pdf"));
                    SmtpClient smtpClient = new SmtpClient
                    {
                        Host = smtp,
                        Port = port,
                        EnableSsl = true,
                        Credentials = new NetworkCredential(from, fromPassword)
                    };

                    await smtpClient.SendMailAsync(mailMessage);
                };

                #endregion Envio correos
            }

            DateTime fileCreationDatetime = DateTime.Now;
            string fileName = string.Format("{0}_{1}.pdf", "Ficha Alta de" + datosAlta.Trabajador, fileCreationDatetime.ToString(@"yyyyMMdd") + "_" + fileCreationDatetime.ToString(@"HHmmss"));

            return File(archivoPDFDescarga, "application/pdf", fileName);
        }



        public void SolicituddePruebasEmail(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.ObtenerDatosPrueba(id, sessione.IdUser);


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;


            if (string.IsNullOrEmpty(datosAlta.CorreosTodaslasSedes))
            {
                datosAlta.CorreosTodaslasSedes = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosSedesProvincia))
            {
                datosAlta.CorreosSedesProvincia = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosSedesLima))
            {
                datosAlta.CorreosSedesLima = "administrador@saluslaboris.com.pe";
            }



            #region NOTIFICACIÓN  SEGÚN MATRIZ
            MailMessage mailMessageNotif = new MailMessage(from, datosAlta.CorreosTodaslasSedes + "," + datosAlta.CorreosSedesProvincia + "," + datosAlta.CorreosSedesLima)
            {
                Subject = "SOLICITUD DE PRUEBA DE ANTIGENOS A LA BREVEDAD Para el Paciente, " + datosAlta.Trabajador,
                IsBodyHtml = true,
                Body = "DNI: " + datosAlta.Dni + "<br>" + "Telefono: " + datosAlta.Telefono + "<br>" + "Direccion: " + datosAlta.Direccion + "<br>" + "Sede: " + datosAlta.NombreSede
            };




            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };


            smtpClient.Send(mailMessageNotif);
            #endregion
        }

        public void SolicituddePruebasEmail5(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.ObtenerDatosPrueba(id, sessione.IdUser);


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;


            if (string.IsNullOrEmpty(datosAlta.CorreosTodaslasSedes))
            {
                datosAlta.CorreosTodaslasSedes = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosSedesProvincia))
            {
                datosAlta.CorreosSedesProvincia = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosSedesLima))
            {
                datosAlta.CorreosSedesLima = "administrador@saluslaboris.com.pe";
            }



            #region NOTIFICACIÓN  SEGÚN MATRIZ
            MailMessage mailMessageNotif = new MailMessage(from, datosAlta.CorreosTodaslasSedes + "," + datosAlta.CorreosSedesProvincia + "," + datosAlta.CorreosSedesLima)
            {
                Subject = "SOLICITUD DE PRUEBA DE ANTIGENOS EN 5 DIAS Para el Paciente, " + datosAlta.Trabajador,
                IsBodyHtml = true,
                Body = "DNI: " + datosAlta.Dni + "<br>" + "Telefono: " + datosAlta.Telefono + "<br>" + "Direccion: " + datosAlta.Direccion + "<br>" + "Sede: " + datosAlta.NombreSede
            };




            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };


            smtpClient.Send(mailMessageNotif);
            #endregion
        }



        public void SolicituddePruebasEmailPulsoOximetro(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.ObtenerDatosPrueba(id, sessione.IdUser);


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;


            if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            {
                datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.MedicoEncargado))
            {
                datosAlta.MedicoEncargado = "administrador@saluslaboris.com.pe";
            }



            #region NOTIFICACIÓN  SEGÚN MATRIZ
            MailMessage mailMessageNotif = new MailMessage(from, datosAlta.CorreoSede + "," + datosAlta.MedicoEncargado)
            {
                Subject = "SOLICITUD DE PULSO OXIMETROS Para el Paciente, " + datosAlta.Trabajador,
                IsBodyHtml = true,
                Body = "DNI: " + datosAlta.Dni + "<br>" + "Telefono: " + datosAlta.Telefono + "<br>" + "Direccion: " + datosAlta.Direccion + "<br>" + "Sede: " + datosAlta.NombreSede
            };




            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };


            smtpClient.Send(mailMessageNotif);
            #endregion
        }




        public void RecetasMedicas(int id, String comentario)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.ObtenerDatosReceta(id, sessione.IdUser);

            if (comentario != null)
            {
                datosAlta.ComentarioAlta = comentario;
            }

            MemoryStream memoryStream = GetPdfReceta(datosAlta);

            if (comentario != null)
            {
                #region Envio correos


                var configEmail = new ReportAltaBL().ParametroCorreo();
                string smtp = configEmail[0].v_Value1.ToLower();
                int port = int.Parse(configEmail[1].v_Value1);
                string from = configEmail[2].v_Value1.ToLower();
                string fromPassword = configEmail[4].v_Value1;

                if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
                {
                    datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
                {
                    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreoSede))
                {
                    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosBP))
                {
                    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
                {
                    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosMedicoZona))
                {
                    datosAlta.CorreosMedicoZona = "saulroach@hotmail.com";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
                {
                    datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
                }

                //SmtpClient smtpClient = new SmtpClient


                //NOTIFICACION RECETA


                //Enviar correo y receta al Trabajador
                MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosMedicoCoord + "," + datosAlta.CorreosMedicoZona)


                //datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios


                {
                    Subject = "RECETA MEDICA",
                    IsBodyHtml = true,
                    Body = "Se adjunta la Receta Médica"
                };

                mailMessage.Attachments.Add(new Attachment(memoryStream, "Receta Medica.pdf"));
                SmtpClient smtpClient = new SmtpClient
                {
                    Host = smtp,
                    Port = port,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(from, fromPassword)
                };

                smtpClient.Send(mailMessage);
                #endregion

            }

        }


        public void EnviarDM(int id, string eid, string Diagnostico)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var empresa = eid;

            if (empresa == "BACKUS" || empresa == "Backus"  || empresa == "UCP" || empresa == "CERVECERIA" || empresa == "San")
            { 
            var datosAlta = oReportAltaBL.EnviarDocumentoDM1(id, sessione.IdUser,Diagnostico);

            //if (comentario != null)
            //{
            //   datosAlta.ComentarioAlta = comentario;
            //}

            MemoryStream memoryStream = GetPdfDescansoMedico(datosAlta);

            //if (comentario != null)
            //{
            #region Envio correos


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
            {
                datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
            }
            //if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
            //{
            //    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            //{
            //    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            //{
            //    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
            //{
            //    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
            //}
            if (string.IsNullOrEmpty(datosAlta.CorreosPeople))
            {
                datosAlta.CorreosPeople = "administrador@saluslaboris.com.pe";
            }
            // if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            //{
            //  datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
            //}

            //SmtpClient smtpClient = new SmtpClient


            //NOTIFICACION RECETA


            //Enviar correo y receta al Trabajador
            MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosPeople)


            //datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios


            {
                Subject = "DESCANSO MEDICO, " + datosAlta.Trabajador,

                IsBodyHtml = true,
                Body = "Estimado trabajador, se envía el descanso médico correspondiente a su vigilancia médica cuyo" + "<br>" + "detalle podrá revisar en el adjunto." + "<br>" + "Recuerde que usted podrá reincorporarse a sus labores sólo a  partir del día siguiente de otorgada" + "<br>" + "el Alta, la cual será comunicada por el médico de vigilancia en su seguimiento telefónico." + "<br>" + "Nota: No es necesario que envíe este descanso médico al área de people service ya que fueron notificados de manera automática."
            };

            mailMessage.Attachments.Add(new Attachment(memoryStream, "Descanso Medico.pdf"));
            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessage);
                #endregion

            }
        }


        public void EnviarDM2(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.EnviarDocumentoDM2(id, sessione.IdUser);

            //if (comentario != null)
            //{
            //   datosAlta.ComentarioAlta = comentario;
            //}

            MemoryStream memoryStream = GetPdfDescansoMedico(datosAlta);

            //if (comentario != null)
            //{
            #region Envio correos


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
            {
                datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
            }
            //if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
            //{
            //    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            //{
            //    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            //{
            //    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
            //{
            //    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
            //}
            if (string.IsNullOrEmpty(datosAlta.CorreosPeople))
            {
                datosAlta.CorreosPeople = "administrador@saluslaboris.com.pe";
            }
            // if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            //{
            //  datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
            //}

            //SmtpClient smtpClient = new SmtpClient


            //NOTIFICACION RECETA


            //Enviar correo y receta al Trabajador
            MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosPeople)


            //datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios


            {
                Subject = "DESCANSO MÉDICO",
                IsBodyHtml = true,
                Body = "Se adjunta el Descanso Médico"
            };

            mailMessage.Attachments.Add(new Attachment(memoryStream, "Descanso Medico.pdf"));
            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessage);
            #endregion

        }


        public void EnviarDM3(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.EnviarDocumentoDM3(id, sessione.IdUser);

            //if (comentario != null)
            //{
            //   datosAlta.ComentarioAlta = comentario;
            //}

            MemoryStream memoryStream = GetPdfDescansoMedico(datosAlta);

            //if (comentario != null)
            //{
            #region Envio correos


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
            {
                datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
            }
            //if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
            //{
            //    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            //{
            //    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            //{
            //    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
            //{
            //    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
            //}
            if (string.IsNullOrEmpty(datosAlta.CorreosPeople))
            {
                datosAlta.CorreosPeople = "administrador@saluslaboris.com.pe";
            }
            // if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            //{
            //  datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
            //}

            //SmtpClient smtpClient = new SmtpClient


            //NOTIFICACION RECETA


            //Enviar correo y receta al Trabajador
            MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosPeople)


            //datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios


            {
                Subject = "DESCANSO MÉDICO",
                IsBodyHtml = true,
                Body = "Se adjunta el Descanso Médico"
            };

            mailMessage.Attachments.Add(new Attachment(memoryStream, "Descanso Medico.pdf"));
            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessage);
            #endregion

        }

        public void EnviarDM4(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.EnviarDocumentoDM4(id, sessione.IdUser);

            //if (comentario != null)
            //{
            //   datosAlta.ComentarioAlta = comentario;
            //}

            MemoryStream memoryStream = GetPdfDescansoMedico(datosAlta);

            //if (comentario != null)
            //{
            #region Envio correos


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
            {
                datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
            }
            //if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
            //{
            //    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            //{
            //    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            //{
            //    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
            //{
            //    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
            //}
            if (string.IsNullOrEmpty(datosAlta.CorreosPeople))
            {
                datosAlta.CorreosPeople = "administrador@saluslaboris.com.pe";
            }
            // if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            //{
            //  datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
            //}

            //SmtpClient smtpClient = new SmtpClient


            //NOTIFICACION RECETA


            //Enviar correo y receta al Trabajador
            MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosPeople)


            //datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios


            {
                Subject = "DESCANSO MÉDICO",
                IsBodyHtml = true,
                Body = "Se adjunta el Descanso Médico"
            };

            mailMessage.Attachments.Add(new Attachment(memoryStream, "Descanso Medico.pdf"));
            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessage);
            #endregion

        }


        public void EnviarDM5(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.EnviarDocumentoDM5(id, sessione.IdUser);

            //if (comentario != null)
            //{
            //   datosAlta.ComentarioAlta = comentario;
            //}

            MemoryStream memoryStream = GetPdfDescansoMedico(datosAlta);

            //if (comentario != null)
            //{
            #region Envio correos


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
            {
                datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
            }
            //if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
            //{
            //    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            //{
            //    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            //{
            //    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
            //{
            //    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
            //}
            if (string.IsNullOrEmpty(datosAlta.CorreosPeople))
            {
                datosAlta.CorreosPeople = "administrador@saluslaboris.com.pe";
            }
            // if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            //{
            //  datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
            //}

            //SmtpClient smtpClient = new SmtpClient


            //NOTIFICACION RECETA


            //Enviar correo y receta al Trabajador
            MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosPeople)


            //datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios


            {
                Subject = "DESCANSO MÉDICO",
                IsBodyHtml = true,
                Body = "Se adjunta el Descanso Médico"
            };

            mailMessage.Attachments.Add(new Attachment(memoryStream, "Descanso Medico.pdf"));
            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessage);
            #endregion

        }

        public void EnviarDM6(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.EnviarDocumentoDM6(id, sessione.IdUser);

            //if (comentario != null)
            //{
            //   datosAlta.ComentarioAlta = comentario;
            //}

            MemoryStream memoryStream = GetPdfDescansoMedico(datosAlta);

            //if (comentario != null)
            //{
            #region Envio correos


            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
            {
                datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
            }
            //if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
            //{
            //    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            //{
            //    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            //{
            //    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            //}
            //if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
            //{
            //    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
            //}
            if (string.IsNullOrEmpty(datosAlta.CorreosPeople))
            {
                datosAlta.CorreosPeople = "administrador@saluslaboris.com.pe";
            }
            // if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            //{
            //  datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
            //}

            //SmtpClient smtpClient = new SmtpClient


            //NOTIFICACION RECETA


            //Enviar correo y receta al Trabajador
            MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosPeople)


            //datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios


            {
                Subject = "DESCANSO MÉDICO",
                IsBodyHtml = true,
                Body = "Se adjunta el Descanso Médico"
            };

            mailMessage.Attachments.Add(new Attachment(memoryStream, "Descanso Medico.pdf"));
            SmtpClient smtpClient = new SmtpClient
            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessage);
            #endregion

        }



        public void GenerarAltaMedica(int id, string comentario)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.ObtenerDatosAlta(id, sessione.IdUser);
            if (comentario != null)
            {
                datosAlta.ComentarioAlta = comentario;
            }

            MemoryStream memoryStream = GetPdfAltaMedica(datosAlta);

            if (comentario != null)
            {
                #region Envio correos

                var configEmail = new ReportAltaBL().ParametroCorreo();
                string smtp = configEmail[0].v_Value1.ToLower();
                int port = int.Parse(configEmail[1].v_Value1);
                string from = configEmail[2].v_Value1.ToLower();
                string fromPassword = configEmail[4].v_Value1;

                if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
                {
                    datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
                {
                    datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreoSede))
                {
                    datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosBP))
                {
                    datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
                {
                    datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosMedicoZona))
                {
                    datosAlta.CorreosMedicoZona = "administrador@saluslaboris.com.pe";
                }
                if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
                {
                    datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
                }


                #region NOTIFICACIÓN  SEGÚN MATRIZ
                MailMessage mailMessageNotif = new MailMessage(from, datosAlta.CorreoSede + "," + datosAlta.CorreosBP + "," + datosAlta.CorreosChampios + "," + datosAlta.CorreosSeguridad + "," + datosAlta.CorreosMedicoZona + "," + datosAlta.CorreosMedicoCoord)
                {
                    Subject = "ALTA VIGILANCIA COVID, " + datosAlta.Trabajador + ", " + datosAlta.FechaAltaMedica,
                    IsBodyHtml = true,
                    Body = "Se informa mediante la presente que el Sr(a) " + datosAlta.Trabajador + "--ha sido dado de alta de la vigilancia médica de COVID en el sistema VIGCOVID, el día " + datosAlta.FechaAltaMedica
                };

                SmtpClient smtpClientNotif = new SmtpClient
                {
                    Host = smtp,
                    Port = port,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(from, fromPassword)
                };

                smtpClientNotif.Send(mailMessageNotif);
                #endregion


                #region ENVIAR CORREO Y ADJUNTAR FICHA ALTA AL TRABAJADOR
                MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador)
                {
                    Subject = "FICHA DE ALTA COVID 19",
                    IsBodyHtml = true,
                    Body = "Se adjunta su ficha de alta"
                };

                mailMessage.Attachments.Add(new Attachment(memoryStream, "ficha de alta.pdf"));
                SmtpClient smtpClient = new SmtpClient
                {
                    Host = smtp,
                    Port = port,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(from, fromPassword)
                };

                smtpClient.Send(mailMessage);
                #endregion



                #endregion Envio correos
            }
        }

        //GENERAR PDF DE RECETAS MEDICAS : Upd : 27-04-2021
        private MemoryStream GetPdfReceta(ReporteAltaBE datos)
        {
            using (Document document = new Document(PageSize.B6)) //Tamaño de Hoja

            {
                MemoryStream memoryStream = new MemoryStream();
                try
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                    pdfWriter.CloseStream = false;
                    pdfWriter.PageEvent = new pdfPage();
                    document.SetMargins(58.34f, 58.34f, 58.34f, 58.34f);
                    document.Open();

                    #region Declaration Tables

                    var subTitleBackGroundColor = new BaseColor(System.Drawing.Color.White);
                    string include = string.Empty;
                    List<PdfPCell> cells = null;
                    float[] columnWidths = null;
                    //string[] columnHeaders = null;
                    PdfPTable filiationWorker = new PdfPTable(8);

                    PdfPTable table = null;

                    PdfPTable whiteline = null;


                    // PdfPCell cell = null;

                    // #endregion Declaration Tables

                    // #region Fonts

                    //Font fontTitle1 = FontFactory.GetFont(FontFactory.HELVETICA, 14, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitle2 = FontFactory.GetFont(FontFactory.HELVETICA, 12, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitleTable = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitleTableNegro = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontSubTitle = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                    //Font fontSubTitleNegroNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontColumnValue = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontColumnValueNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontAptitud = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));


                    Font fontTitle1 = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitle2 = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitleTable = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitleTableNegro = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontSubTitle = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                    Font fontSubTitleNegroNegrita = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontColumnValue = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    Font fontColumnValueNegrita = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontAptitud = FontFactory.GetFont(FontFactory.TIMES, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));


                    #endregion Fonts

                    #region Title

                    cells = new List<PdfPCell>();

                    var fechaHoy = DateTime.Now;

                    var fechaFormat = fechaHoy.ToString("dd/MM/yyyy");


                    //columnWidths = new float[] { 30f, 70f, 30f };
                    columnWidths = new float[] { 50f, 50f, 30f };
                    var logo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    var cellsTit = new List<PdfPCell>()
                    {
                        new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(logo),null,null,80,20)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE, FixedHeight = 50f },
                        new PdfPCell(new Phrase("RECETA MEDICA", fontTitle1)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE },
                        new PdfPCell(new Phrase(fechaFormat, fontTitle1)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE },


                        new PdfPCell(new Phrase(datos.Receta, fontTitle2)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6},
                        //new PdfPCell(new Phrase(datos.Receta, fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},
                        //new PdfPCell(new Phrase(datos.Receta, fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                    };

                    table = HandlingItextSharp.GenerateTableFromCells(cellsTit, columnWidths, null, fontTitleTable);

                    document.Add(table);

                    cells = new List<PdfPCell>();

                    //columnWidths = new float[] { 50f, 50f };
                    columnWidths = new float[] { 100f };
                    string firma;

                    string path = Path.Combine(HttpRuntime.AppDomainAppPath, "img");
                    string fileName = datos.DoctorId.ToString() + ".png";

                    if (!System.IO.File.Exists(Path.Combine(path, fileName)))
                    {
                        firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    }
                    else
                    {
                        firma = System.IO.Path.Combine(path, fileName);
                    }


                    // var firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");

                    //firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\" + datos.DoctorId.ToString() + ".png");

                    //if (string.IsNullOrEmpty(firma))
                    //{
                    //    firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    //}





                    //columnWidths = new float[] { 30f, 70f, 30f };
                    columnWidths = new float[] { 100f };

                    var Firma = new List<PdfPCell>()
                    {
                        new PdfPCell(new Phrase("Firma del médico responsable", fontSubTitleNegroNegrita)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE, Border = PdfPCell.NO_BORDER},

                        new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(firma),null,null,100,40)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE, FixedHeight = 50f,Border = PdfPCell.NO_BORDER},


                        new PdfPCell(new Phrase(datos.DatosDoctor + "\n" + "C.M.P " + datos.Colegiatura  , fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER},


                        // new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(firma),null,null,100,40)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE , Border = PdfPCell.NO_BORDER},

                        //new PdfPCell(new Phrase("Nombre del médico responsable - CMP " + datos.Colegiatura, fontSubTitle)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE},

                       // ----> new PdfPCell(new Phrase("Firma del médico responsable", fontSubTitleNegroNegrita)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE, Border = PdfPCell.NO_BORDER},
                    };
                    table = HandlingItextSharp.GenerateTableFromCells(Firma, columnWidths, null, fontTitleTable);

                    document.Add(table);

                    #endregion Firma Médico

                    document.NewPage();

                    document.Close();

                    byte[] byteInfo = memoryStream.ToArray();
                    memoryStream.Write(byteInfo, 0, byteInfo.Length);
                    memoryStream.Position = 0;
                }
                catch (Exception ex)
                {
                    throw;
                }
                return memoryStream;
            }
        }


        //GENERAR PDF DE DESCANSO MEDICO

        private MemoryStream GetPdfDescansoMedico(ReporteAltaBE datos)
        {
            using (Document document = new Document(PageSize.A4)) //Tamaño de Hoja

            {
                MemoryStream memoryStream = new MemoryStream();
                try
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                    pdfWriter.CloseStream = false;
                    pdfWriter.PageEvent = new pdfPage();
                    document.SetMargins(58.34f, 58.34f, 58.34f, 58.34f);
                    document.Open();

                    #region Declaration Tables

                    var subTitleBackGroundColor = new BaseColor(System.Drawing.Color.White);
                    string include = string.Empty;
                    List<PdfPCell> cells = null;
                    float[] columnWidths = null;
                    //string[] columnHeaders = null;
                    PdfPTable filiationWorker = new PdfPTable(8);

                    PdfPTable table = null;

                    PdfPTable whiteline = null;


                    // PdfPCell cell = null;

                    // #endregion Declaration Tables

                    // #region Fonts

                    //Font fontTitle1 = FontFactory.GetFont(FontFactory.HELVETICA, 14, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitle2 = FontFactory.GetFont(FontFactory.HELVETICA, 12, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitleTable = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitleTableNegro = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontSubTitle = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                    //Font fontSubTitleNegroNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontColumnValue = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontColumnValueNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontAptitud = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));


                    Font fontTitle1 = FontFactory.GetFont(FontFactory.HELVETICA, 15, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitle2 = FontFactory.GetFont(FontFactory.HELVETICA, 11, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitle44 = FontFactory.GetFont(FontFactory.HELVETICA, 13, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitleTable = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitleTableNegro = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontSubTitle = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                    Font fontSubTitleNegroNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontColumnValue = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    Font fontColumnValueNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontAptitud = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));


                    #endregion Fonts

                    #region Title

                    cells = new List<PdfPCell>();

                    var fechaHoy = DateTime.Now;

                    var fechaFormat = fechaHoy.ToString("dd/MM/yyyy");


                    //columnWidths = new float[] { 30f, 70f, 30f };
                    columnWidths = new float[] { 100f };
                    var logo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    var cellsTit = new List<PdfPCell>()
                    {
                        new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(logo),null,null,150,36)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE, FixedHeight = 50f,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("DESCANSO MÉDICO", fontTitle1)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        //new PdfPCell(new Phrase(fechaFormat, fontTitle1)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase(" " + "El médico que suscribe certifica que:", fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase(" " + "El(la)  señor(a):" +" "+ datos.Trabajador +" " + "con" + " " + " DNI Nro. " + "    " + datos.Dni + " ", fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase(" " + "de la empresa Unión de Cervecerías Peruana Backus y Johnston S.A.A., quien" + " " + "luego de la", fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase(" " + "evaluación correspondiente presenta diagnóstico de:",fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase("  " + "   " + "-" + datos.Diagnostico,fontTitle44)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        //new PdfPCell(new Phrase("-Z29.0 Aislamiento",fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },

                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase(" " + "Por lo que se indica" + " " + " " + datos.DiasTotalDescanso + " " + " " + "días de descanso médico, del" + " " +  " " + datos.FechaAislaminetoCuarentena + " " + "al" + " " + datos.FechaPosibleAlta +" " + "se emite" + " " + "el", fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase(" " + "presente certificado a solicitud  del interesado y para los fines que crea conveniente",fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                        //new PdfPCell(new Phrase(" " + "conveniente",fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},
                        new PdfPCell(new Phrase("luego de la evaluación correspondiente presenta diagnóstico de:", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},


                        new PdfPCell(new Phrase("Lima" +" "+ " " + datos.FechaAislaminetoCuarentena,fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER },


                        //new PdfPCell(new Phrase(datos.Receta, fontTitle2)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6},
                        //new PdfPCell(new Phrase(datos.Receta, fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},
                        //new PdfPCell(new Phrase(datos.Receta, fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6,Border = PdfPCell.NO_BORDER},

                    };

                    table = HandlingItextSharp.GenerateTableFromCells(cellsTit, columnWidths, null, fontTitleTable);

                    document.Add(table);

                    cells = new List<PdfPCell>();

                    //columnWidths = new float[] { 50f, 50f };
                    columnWidths = new float[] { 100f };
                    string firma;

                    string path = Path.Combine(HttpRuntime.AppDomainAppPath, "img");
                    string fileName = datos.DoctorId.ToString() + ".png";

                    if (!System.IO.File.Exists(Path.Combine(path, fileName)))
                    {
                        firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    }
                    else
                    {
                        firma = System.IO.Path.Combine(path, fileName);
                    }


                    // var firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");

                    //firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\" + datos.DoctorId.ToString() + ".png");

                    //if (string.IsNullOrEmpty(firma))
                    //{
                    //    firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    //}





                    //columnWidths = new float[] { 30f, 70f, 30f };
                    columnWidths = new float[] { 100f };

                    var Firma = new List<PdfPCell>()
                    {
                        //new PdfPCell(new Phrase("Firma del médico responsable", fontSubTitleNegroNegrita)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE, Border = PdfPCell.NO_BORDER},

                        new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(firma),null,null,100,40)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE, FixedHeight = 50f,Border = PdfPCell.NO_BORDER},


                        new PdfPCell(new Phrase(datos.DatosDoctor + "\n" + "C.M.P " + datos.Colegiatura  , fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER},

                        new PdfPCell(new Phrase("SALUS LABORIS SAC - USE BACKUS", fontSubTitleNegroNegrita)){ HorizontalAlignment = PdfPCell.ALIGN_LEFT,VerticalAlignment = PdfPCell.ALIGN_MIDDLE, Border = PdfPCell.NO_BORDER},

                        // new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(firma),null,null,100,40)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE , Border = PdfPCell.NO_BORDER},

                        //new PdfPCell(new Phrase("Nombre del médico responsable - CMP " + datos.Colegiatura, fontSubTitle)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE},

                       // ----> new PdfPCell(new Phrase("Firma del médico responsable", fontSubTitleNegroNegrita)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE, Border = PdfPCell.NO_BORDER},
                    };
                    table = HandlingItextSharp.GenerateTableFromCells(Firma, columnWidths, null, fontTitleTable);

                    document.Add(table);

                    #endregion Firma Médico

                    document.NewPage();

                    document.Close();

                    byte[] byteInfo = memoryStream.ToArray();
                    memoryStream.Write(byteInfo, 0, byteInfo.Length);
                    memoryStream.Position = 0;
                }
                catch (Exception ex)
                {
                    throw;
                }
                return memoryStream;
            }
        }







        //GENERAR PDF DE ALTAS MEDICAS - FICHA DE ALTA
        private MemoryStream GetPdfAltaMedica(ReporteAltaBE datos)
        {
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream memoryStream = new MemoryStream();
                try
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                    pdfWriter.CloseStream = false;
                    pdfWriter.PageEvent = new pdfPage();
                    document.SetMargins(38.34f, 38.34f, 38.34f, 38.34f);
                    document.Open();

                    #region Declaration Tables

                    var subTitleBackGroundColor = new BaseColor(System.Drawing.Color.White);
                    string include = string.Empty;
                    List<PdfPCell> cells = null;
                    float[] columnWidths = null;
                    string[] columnHeaders = null;
                    PdfPTable filiationWorker = new PdfPTable(8);

                    PdfPTable table = null;

                    PdfPTable whiteline = null;


                    PdfPCell cell = null;

                    #endregion Declaration Tables

                    #region Fonts

                    //Font fontTitle1 = FontFactory.GetFont(FontFactory.HELVETICA, 14, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitle2 = FontFactory.GetFont(FontFactory.HELVETICA, 12, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitleTable = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontTitleTableNegro = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontSubTitle = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                    //Font fontSubTitleNegroNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontColumnValue = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    //Font fontColumnValueNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    //Font fontAptitud = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));


                    Font fontTitle1 = FontFactory.GetFont(FontFactory.HELVETICA, 12, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitle2 = FontFactory.GetFont(FontFactory.HELVETICA, 12, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitleTable = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontTitleTableNegro = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontSubTitle = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.White));
                    Font fontSubTitleNegroNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontColumnValue = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Black));
                    Font fontColumnValueNegrita = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));
                    Font fontAptitud = FontFactory.GetFont(FontFactory.HELVETICA, 9, iTextSharp.text.Font.BOLD, new BaseColor(System.Drawing.Color.Black));


                    #endregion Fonts

                    #region Title

                    cells = new List<PdfPCell>();

                    //columnWidths = new float[] { 30f, 70f, 30f };
                    columnWidths = new float[] { 45f, 60f, 30f };
                    var logo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    var cellsTit = new List<PdfPCell>()
                    {
                        new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(logo),null,null,150,36)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE, FixedHeight = 50f },
                        new PdfPCell(new Phrase("ALTA EPIDEMIOLÓGICA COVID-19", fontTitle1)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE },
                        new PdfPCell(new Phrase("FORM-SL-131 Versión: 05", fontTitleTable)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE },
                    };

                    table = HandlingItextSharp.GenerateTableFromCells(cellsTit, columnWidths, null, fontTitleTable);

                    document.Add(table);

                    #endregion Title

                    #region Datos Personales

                    cells = new List<PdfPCell>()
                   {
                    //fila
                    new PdfPCell(new Phrase("Apellidos y Nombres", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.Trabajador, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},

                    //fila
                    new PdfPCell(new Phrase("Dni", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT},
                    new PdfPCell(new Phrase(datos.Dni, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT},
                    new PdfPCell(new Phrase("Edad", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT},
                    new PdfPCell(new Phrase(datos.Edad.ToString(), fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT},
                    new PdfPCell(new Phrase("Sexo", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT},
                    new PdfPCell(new Phrase(datos.Sexo, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT},


                    new PdfPCell(new Phrase(datos.Sexo, fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=6},


                     //fila
                    //new PdfPCell(new Phrase("Empresa Empleadora", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=1},
                    //new PdfPCell(new Phrase(datos.Empresa, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=2},
                    //new PdfPCell(new Phrase("Puesto", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=1},
                    //new PdfPCell(new Phrase(datos.PuestoTrabajo, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=2},


                    new PdfPCell(new Phrase("Empresa Empleadora", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.Empresa, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase("Puesto", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.PuestoTrabajo, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},







                    //fila
                    new PdfPCell(new Phrase("Motivo de la vigilancia epidemiológica", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.ModoIngreso.ToString(), fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},

                    //fila
                    new PdfPCell(new Phrase("Vía de ingreso a la vigilancia médica COVID", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.ViaIngreso.ToString(), fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},

                    //fila
                    new PdfPCell(new Phrase("Fecha en que ingresó a la vigilancia médica COVID", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.FechaRegistro.ToString("dd/MMMM/yyyy"), fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},


                    };

                    columnWidths = new float[] { 16f, 16f, 16f, 16f, 16f, 16f };

                    filiationWorker = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, "DATOS DEL TRABAJADOR", fontTitleTable);

                    document.Add(filiationWorker);

                    #endregion Datos Personales

                    #region Resultados de pruebas

                    cells = new List<PdfPCell>();

                    var ent = datos.Examenes.ToList();
                    if (ent != null && ent.Count > 0)
                    {
                        foreach (var item in ent)
                        {
                            cell = new PdfPCell(new Phrase(item.TipoPrueba.ToString(), fontColumnValue)) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                            cells.Add(cell);
                            cell = new PdfPCell(new Phrase(item.Fecha.ToString("dd/MMMM/yyyy"), fontColumnValue)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE };
                            cells.Add(cell);
                            cell = new PdfPCell(new Phrase(item.Resultado.ToString(), fontColumnValue)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE };
                            cells.Add(cell);
                        }
                        columnWidths = new float[] { 40f, 30f, 30f };
                    }
                    else
                    {
                        cells.Add(new PdfPCell(new Phrase("No se han registrado datos.", fontColumnValue)) { Colspan = 8, HorizontalAlignment = PdfPCell.ALIGN_LEFT });
                        columnWidths = new float[] { 100f };
                    }
                    columnHeaders = new string[] { "Tipo de Examen", "Fecha", "Resultado" };

                    var grillaExamenes = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, "RESULTADO DE PRUEBAS PARA EL DIAGNÓSTICO DE COVID-19", fontTitleTable, columnHeaders);
                    document.Add(grillaExamenes);

                    #endregion Resultados de pruebas

                    #region Conclusiones
                    cells = new List<PdfPCell>()
                   {
                        new PdfPCell(new Phrase("Fecha de alta médica", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                          new PdfPCell(new Phrase(datos.FechaAltaMedica, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    //fila
                    new PdfPCell(new Phrase("Número de días de cuarentena / aislamiento", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.DiasCuarentena, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},

                    //fila
                    new PdfPCell(new Phrase("Comentario para el alta", fontColumnValueNegrita)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},
                    new PdfPCell(new Phrase(datos.ComentarioAlta, fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT, Colspan=3},

                    new PdfPCell(new Phrase("EL MEDICO QUE SUSCRIBE CERTIFICA EL ALTA EPIDEMIOLÓGICA DE LA VIGILANCIA MÉDICA POR CAUSA RELACIONADA A COVID-19 acorde a las recomendaciones técnicas y normativas (RM-193-2020-MINSA y Modificatoria RM-375-2020-MINSA, RM-448-2020-MINSA)", fontColumnValue)) { HorizontalAlignment = PdfPCell.ALIGN_JUSTIFIED, Colspan=6, Rowspan=2},

                    new PdfPCell(new Phrase("EL MEDICO QUE SUSCRIBE CERTIFICA EL ALTA EPIDEMIOLÓGICA DE LA VIGILANCIA MÉDICA POR CAUSA RELACIONADA A COVID-19 acorde a las recomendaciones técnicas y normativas (RM-193-2020-MINSA y Modificatoria RM-375-2020-MINSA, RM-448-2020-MINSA)", fontSubTitle)) { HorizontalAlignment = PdfPCell.ALIGN_JUSTIFIED,Colspan=6, Border = PdfPCell.NO_BORDER},

                };




                    columnWidths = new float[] { 16f, 16f, 16f, 16f, 16f, 16f };

                    var conclusiones = HandlingItextSharp.GenerateTableFromCells(cells, columnWidths, "CONCLUSIONES PARA EL ALTA EPIDEMIOLÓGICA", fontTitleTable);

                    document.Add(conclusiones);

                    #endregion Conclusiones

                    #region Firma Médico

                    cells = new List<PdfPCell>();

                    //columnWidths = new float[] { 50f, 50f };
                    columnWidths = new float[] { 100f };
                    string firma;

                    string path = Path.Combine(HttpRuntime.AppDomainAppPath, "img");
                    string fileName = datos.DoctorId.ToString() + ".png";

                    if (!System.IO.File.Exists(Path.Combine(path, fileName)))
                    {
                        firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    }
                    else
                    {
                        firma = System.IO.Path.Combine(path, fileName);
                    }


                    // var firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");

                    //firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\" + datos.DoctorId.ToString() + ".png");

                    //if (string.IsNullOrEmpty(firma))
                    //{
                    //    firma = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"img\logoSalus.jpg");
                    //}





                    //columnWidths = new float[] { 30f, 70f, 30f };
                    columnWidths = new float[] { 100f };

                    var Firma = new List<PdfPCell>()
                    {
                        new PdfPCell(new Phrase("Firma del médico responsable", fontSubTitleNegroNegrita)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE, Border = PdfPCell.NO_BORDER},

                        new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(firma),null,null,100,40)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE, FixedHeight = 50f,Border = PdfPCell.NO_BORDER},


                        new PdfPCell(new Phrase(datos.DatosDoctor + "\n" + "C.M.P " + datos.Colegiatura  , fontTitle2)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE,Border = PdfPCell.NO_BORDER},


                        // new PdfPCell(HandlingItextSharp.GetImage(VigCovid.Common.Resource.Utils.FileToByteArray(firma),null,null,100,40)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE , Border = PdfPCell.NO_BORDER},

                        //new PdfPCell(new Phrase("Nombre del médico responsable - CMP " + datos.Colegiatura, fontSubTitle)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE},

                       // ----> new PdfPCell(new Phrase("Firma del médico responsable", fontSubTitleNegroNegrita)){ HorizontalAlignment = PdfPCell.ALIGN_CENTER,VerticalAlignment = PdfPCell.ALIGN_MIDDLE, Border = PdfPCell.NO_BORDER},
                    };
                    table = HandlingItextSharp.GenerateTableFromCells(Firma, columnWidths, null, fontTitleTable);

                    document.Add(table);

                    #endregion Firma Médico

                    document.NewPage();

                    document.Close();

                    byte[] byteInfo = memoryStream.ToArray();
                    memoryStream.Write(byteInfo, 0, byteInfo.Length);
                    memoryStream.Position = 0;
                }
                catch (Exception ex)
                {
                    throw;
                }
                return memoryStream;
            }
        }
    }
}
