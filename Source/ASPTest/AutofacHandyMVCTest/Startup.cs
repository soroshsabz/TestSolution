using Autofac;
using Autofac.Configuration;

namespace AutofacHandyMVCTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("autofac.json");
            var autoFacConfigurationModule = new ConfigurationModule(configurationBuilder.Build());

            builder.RegisterModule(autoFacConfigurationModule);
        }

        public void ConfigureApp<App>(App app, IWebHostEnvironment env) where App : IApplicationBuilder, IEndpointRouteBuilder, IHost
        {
            // Configure the HTTP request pipeline.
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        }
    }
}
