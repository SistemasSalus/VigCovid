using System;
using System.Web.Mvc;
using System.Web.Security;
using VigCovidApp.Models;
using VigCovidApp.Security;

namespace VigCovidApp.Controllers.Base
{
    [AuthorizeFilter]
    public class GenericController : Controller
    {
        private SessionModel _sessionUsuario;

        public GenericController()
        {
        }

        public GenericController(SessionModel sessionModel)
        {
            SessionUsuario = sessionModel;
        }

        public SessionModel SessionUsuario
        {
            get
            {
                return _sessionUsuario ?? HttpSessionContext.CurrentAccount();
            }
            private set
            {
                _sessionUsuario = value;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (SessionUsuario == null)
                {
                    string urlSignOut = GetUrlLogoutSession();
                    filterContext.Result = new RedirectResult(urlSignOut);
                    return;
                }

                GetUserDataViewBag();

                base.OnActionExecuting(filterContext);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void GetUserDataViewBag()
        {
            ViewBag.SessionTimeout = HttpContext.Session.Timeout;
        }

        private string GetUrlLogoutSession()
        {
            string URLSignOut = string.Empty;
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains(Request.Url.Host))
                URLSignOut = "/AccessSystem/SesionExpirada";
            else
                URLSignOut = "/AccessSystem/UserUnknown";

            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return URLSignOut;
        }

    }
}



//#endregion Class

//public FileResult ExportAltaMedica(int id, string comentario)
//{
//    var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
//    var oReportAltaBL = new ReportAltaBL();
//    var datosAlta = oReportAltaBL.ObtenerDatosAlta(id, sessione.IdUser);
//    if (comentario != null)
//    {
//        datosAlta.ComentarioAlta = comentario;
//    }

//    MemoryStream memoryStream = GetPdfAltaMedica(datosAlta);

//    if (comentario == null)
//    {
//        #region Envio correos

//        var configEmail = new ReportAltaBL().ParametroCorreo();
//        string smtp = configEmail[0].v_Value1.ToLower();
//        int port = int.Parse(configEmail[1].v_Value1);
//        string from = configEmail[2].v_Value1.ToLower();
//        string fromPassword = configEmail[4].v_Value1;
//        if (datosAlta.CorreosTrabajador == null)
//        {
//            datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
//        }
//        if (datosAlta.CorreosChampios == null)
//        {
//            datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
//        }



//        MailMessage mailMessage = new MailMessage(from, datosAlta.CorreosTrabajador + "," + datosAlta.CorreosChampios)
//        {
//            Subject = "FICHA DE ALTA COVID 19",
//            IsBodyHtml = true,
//            Body = "Se adjunta su ficha de alta"
//        };

//        mailMessage.Attachments.Add(new Attachment(memoryStream, "ficha de alta.pdf"));
//        SmtpClient smtpClient = new SmtpClient
//        {
//            Host = smtp,
//            Port = port,
//            EnableSsl = true,
//            Credentials = new NetworkCredential(from, fromPassword)
//        };

//        smtpClient.Send(mailMessage);

//    #endregion Envio correos
//    }

//DateTime fileCreationDatetime = DateTime.Now;
//    string fileName = string.Format("{0}_{1}.pdf", "Ficha Alta de" + datosAlta.Trabajador, fileCreationDatetime.ToString(@"yyyyMMdd") + "_" + fileCreationDatetime.ToString(@"HHmmss"));

//    return File(memoryStream, "application/pdf", fileName);
//}


