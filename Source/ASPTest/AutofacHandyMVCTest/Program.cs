using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace AutofacHandyMVCTest
{
    /// <summary>
    /// Based on https://learn.microsoft.com/en-us/aspnet/core/fundamentals/apis?view=aspnetcore-6.0
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var startup = new Startup(builder.Configuration);


            // Add services to the container.
            startup.ConfigureServices(builder.Services);

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(startup.ConfigureContainer);


            WebApplication app = builder.Build();

            startup.ConfigureApp(app, app.Environment);

            app.Run();
        }
    }
}