using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using VigCovid.Security;
using VigCovidApp.Models;
using VigCovidApp.Security;
using VigCovidApp.ViewModels;

namespace VigCovidApp.Controllers
{
    public class AccessSystemController : Controller
    {
        // GET: AccessSystem
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ListarEmpresas();
            return View();
        }

        public void ListarEmpresas()
        {
            ViewBag.EMPRESAS = new Empresas().ListarEmpresas();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {//Agregado por Saul 05082021 -  Aceptar empresas segun el Usuario registrado
                var empresasAsignadas = ValidateUser(model.Username.Trim().ToLower(), model.Password).ToList();
                if (empresasAsignadas.Count() >= 1)
                {
                    var permisos = new AccessBL().PermisoPorTipoUsuario(empresasAsignadas[0].TipoUsuarioId);
                    var sessionModel = new SessionModel();
                    sessionModel.IdUser = empresasAsignadas[0].UsuarioId;
                    sessionModel.IdTipoUsuario = empresasAsignadas[0].TipoUsuarioId;
                    sessionModel.UserName = model.Username;
                    sessionModel.EmpresasAsignadas = empresasAsignadas;
                    sessionModel.EmpresaId = empresasAsignadas[0].EmpresaId; //Agregado por Saul 13052021

                    sessionModel.AsignarPacientes = permisos.AsignarPacientes;
                    sessionModel.PacientesAsignados = permisos.PacientesAsignados;
                    sessionModel.AccesoOtrosPacientesLectura = permisos.AccesoOtrosPacientesLectura;
                    sessionModel.AccesoOtrosPacientesModificacion = permisos.AccesoOtrosPacientesModificacion;

                    FormsAuthentication.SetAuthCookie(sessionModel.UserName, false);

                    HttpSessionContext.SetAccount(sessionModel);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ListarEmpresas();
                    ModelState.AddModelError("", "Contraseña o identificador de usuario incorrectos. Escriba la contraseña y el identificador de usuario correctos e inténtelo de nuevo.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ListarEmpresas();
                ModelState.AddModelError("", "Error al procesar la solicitud.");
                return RedirectToAction("Login", "AccessSystem");
            }
        }

        private List<EmpresaSedeViewModel> ValidateUser(string username, string password)
        {
            var oAccessBL = new AccessBL();
            var accesos = new List<EmpresaSedeViewModel>();
            var usuarioDB = oAccessBL.ValidarUsuario(username, password);

            if (usuarioDB != null)
            {
                var accesosInformacion = oAccessBL.ObtenerEmpresaSedesPorUsuario(usuarioDB.Id, usuarioDB.TipoUsuarioId).ToList();
                foreach (var item in accesosInformacion)
                {
                    foreach (var sede in item.Sedes)
                    {
                        var oEmpresaSedeViewModel = new EmpresaSedeViewModel();
                        oEmpresaSedeViewModel.UsuarioId = usuarioDB.Id;
                        oEmpresaSedeViewModel.TipoUsuarioId = usuarioDB.TipoUsuarioId;
                        oEmpresaSedeViewModel.EmpresaIdSedeId = item.EmpresaId + "-" + sede.SedeId;
                        oEmpresaSedeViewModel.EmpresaSede = item.EmpresaNombre + "-" + sede.SedeNombre;
                        oEmpresaSedeViewModel.EmpresaId = item.EmpresaId; //añadido para varias empresas por saul 13052021
                        accesos.Add(oEmpresaSedeViewModel);
                    }
                }

                return accesos.GroupBy(g => g.EmpresaIdSedeId).Select(s => s.First()).ToList();
            }

            return null;
        }

        [AllowAnonymous]
        public ActionResult UserUnknown()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            return SignOff();
        }

        private ActionResult SignOff()
        {
            var rolUsuario = string.Empty;
            var userData = Security.HttpSessionContext.CurrentAccount();
            if (userData != null)
            {
                rolUsuario = userData.UserName;
            }

            var urlSignOut = string.Empty;

            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.Abandon();
            System.Web.HttpContext.Current.Session.Clear();

            urlSignOut = string.Format("{0}", FormsAuthentication.LoginUrl);

            return Redirect(urlSignOut);
        }

        [AllowAnonymous]
        public ActionResult SesionExpirada(string returnUrl)
        {
            AsignarUrlRetorno(returnUrl);
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Generals");
        }

        protected virtual void AsignarUrlRetorno(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
        }
    }
}