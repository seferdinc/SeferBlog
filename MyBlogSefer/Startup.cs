using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyBlogSefer.Startup))]
namespace MyBlogSefer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
