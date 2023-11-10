using Autofac;
using Autofac.Configuration;
using Autofac.Features.AttributeFilters;
using AutofacHandyMVCTest.Controllers;
using AutofacHandyMVCTest.Models;
using Microsoft.AspNetCore.Mvc;

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
            services.AddMvc().AddControllersAsServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("autofac.json");
            var autoFacConfigurationModule = new ConfigurationModule(configurationBuilder.Build());

            builder.RegisterModule(autoFacConfigurationModule);

            //builder.RegisterType<DummyA>().Keyed<IDummyModel>(nameof(DummyA)).SingleInstance();
            //builder.RegisterType<DummyB>().Keyed<IDummyModel>(nameof(DummyB)).SingleInstance();
            //builder.RegisterType<HomeController>().WithAttributeFiltering();
            //var controllers = typeof(Startup).Assembly.GetTypes().Where(t => t.BaseType == typeof(ControllerBase)).ToArray(); // for api controller
            var controllers = typeof(Startup).Assembly.GetTypes().Where(t => t.BaseType == typeof(Controller)).ToArray(); // for mvc controller
            builder.RegisterTypes(controllers).WithAttributeFiltering();
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
