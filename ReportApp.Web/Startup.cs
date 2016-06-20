using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReportApp.Web.Startup))]
namespace ReportApp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
