using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Absen.Startup))]
namespace Absen
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
