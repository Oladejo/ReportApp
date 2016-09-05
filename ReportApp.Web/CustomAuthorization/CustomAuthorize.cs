using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ReportApp.Web.CustomAuthorization
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        public string View { get; set; }

        public CustomAuthorize()
        {
            View = "AuthorizedFailed";
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            IsUserAuthorized(filterContext);
        }

        private void IsUserAuthorized(AuthorizationContext filterContext)
        {
            if (filterContext.Result == null && ( filterContext.HttpContext.User.IsInRole("Admin") 
                || filterContext.HttpContext.User.Identity.GetUserName() == "admin@project.com"))
                return;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {   
                var vr = new ViewResult {ViewName = View};
                var dict = new ViewDataDictionary { { "Message", "Sorry you are not Authorized to Perform this Action" } };
                vr.ViewData = dict;
                var result = vr;
                filterContext.Result = result;
            }
        }
    }
}