using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HFGitAccessWeb.Startup))]
namespace HFGitAccessWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
