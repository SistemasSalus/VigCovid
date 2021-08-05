using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VigCovidApp.Models;

namespace VigCovidApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            if (String.Compare(Request.Path, Request.ApplicationPath, StringComparison.InvariantCultureIgnoreCase) == 0
                && !(Request.Path.EndsWith("/")))
                Response.Redirect(string.Format("{0}/", Request.Path));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            if (HttpContext.Current != null && HttpContext.Current.Session[Resources.Constants.SessionUser] != null)
            {
                var sessionUsuario = (SessionModel)HttpContext.Current.Session[Resources.Constants.SessionUser];
            }
            else
            {
                //ErrorUtilities.AddLog(exception, "", "");
            }
        }
    }
}