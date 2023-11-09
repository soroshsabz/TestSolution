using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace AutofacHandyTest
{
    public class Program
    {
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