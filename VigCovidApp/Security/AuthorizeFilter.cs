using System.Web.Mvc;

namespace VigCovidApp.Security
{
    public class AuthorizeFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpSessionContext.CurrentAccount() == null)
            {
                filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl);
                filterContext.Result.ExecuteResult(filterContext);
            }
        }
    }
}