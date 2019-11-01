using Autofac;
using Autofac.Integration.WebApi;
using FortuneTellerService4.Models;
using Microsoft.Owin;
using Owin;
using Steeltoe.Common.Options.Autofac;
using Steeltoe.Discovery.Client;
using System.Reflection;

[assembly: OwinStartup(typeof(FortuneTellerOwinService4.Startup))]

namespace FortuneTellerOwinService4
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpConfig = WebApiConfig.RegisterRoutes(app);

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterOptions();
            builder.RegisterDiscoveryClient(ApplicationConfig.Configuration);

            // Initialize and Register FortuneContext
            builder.RegisterInstance(SampleData.InitializeFortunes()).SingleInstance();

            // Register FortuneRepository
            builder.RegisterType<FortuneRepository>().As<IFortuneRepository>().SingleInstance();

            var container = builder.Build();
            app.UseAutofacMiddleware(container);
            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            container.StartDiscoveryClient();
        }
    }
}
