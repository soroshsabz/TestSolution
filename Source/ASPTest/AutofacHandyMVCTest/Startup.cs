using Autofac;
using Autofac.Configuration;
using Autofac.Features.AttributeFilters;
using AutofacHandyMVCTest.Controllers;
using AutofacHandyMVCTest.Models;
using ClassLibraryTestMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Swashbuckle.AspNetCore.Filters;

namespace AutofacHandyMVCTest
{
    /// <summary>
    /// Based on <see href="https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60?#use-startup-with-the-new-minimal-hosting-model"/>
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The application configuration property (appsettings.json)
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure the application services.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();
            services.AddApiVersioning((opt) =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                new HeaderApiVersionReader("x-api-version"),
                                                                new MediaTypeApiVersionReader("x-api-version"));
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerOptions>();

            // based on https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters?tab=readme-ov-file#installation
            services.AddSwaggerExamplesFromAssemblyOf<TestDataViewModelResponseErrorExample>();
        }

        /// <summary>
        /// Configure the application container.
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("autofac.json");
            var autoFacConfigurationModule = new ConfigurationModule(configurationBuilder.Build());

            builder.RegisterModule(autoFacConfigurationModule);

            var controllers = typeof(Startup).Assembly.GetTypes().Where(t => t.BaseType == typeof(Controller)).ToArray(); // for mvc controller
            builder.RegisterTypes(controllers).WithAttributeFiltering();

            controllers = typeof(Startup).Assembly.GetTypes().Where(t => t.BaseType == typeof(ControllerBase)).ToArray(); // for api controller
            builder.RegisterTypes(controllers).WithAttributeFiltering();
        }

        /// <summary>
        /// Configure the application and the HTTP request pipeline.
        /// </summary>
        /// <typeparam name="App"></typeparam>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void ConfigureApp<App>(App app, IWebHostEnvironment env) where App : IApplicationBuilder, IEndpointRouteBuilder, IHost
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });
            }
            else
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
