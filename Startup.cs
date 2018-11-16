using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Automated_LinkedIn_Functionality.Startup))]
namespace Automated_LinkedIn_Functionality
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
