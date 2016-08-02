using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(ParcelXpress.Startup))]

namespace ParcelXpress
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("SimpleConnectionString");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
