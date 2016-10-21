using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FE.Creator.Admin.Startup))]
namespace FE.Creator.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
