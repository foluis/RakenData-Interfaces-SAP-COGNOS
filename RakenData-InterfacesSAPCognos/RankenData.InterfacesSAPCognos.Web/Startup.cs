using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RankenData.InterfacesSAPCognos.Web.Startup))]
namespace RankenData.InterfacesSAPCognos.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
