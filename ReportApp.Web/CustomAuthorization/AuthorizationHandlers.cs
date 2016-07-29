using System.Web.Mvc;

namespace ReportApp.Web.CustomAuthorization
{
    public class ClaimAuthorization : AuthorizeAttribute
    {
        public string View { get; set; }

        public ClaimAuthorization()
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
            if (filterContext.Result == null && (filterContext.HttpContext.User.IsInRole("Unit")
                || filterContext.HttpContext.User.IsInRole("Department")))
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