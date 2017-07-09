using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TestMobileAppForAuth.Startup))]

namespace TestMobileAppForAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}