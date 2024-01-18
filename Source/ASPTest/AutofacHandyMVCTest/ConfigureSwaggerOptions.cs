using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace AutofacHandyMVCTest
{
    /// <summary>
    /// Configure Swagger Options
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider"></param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Configure each API discovered for Swagger Documentation
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options)
        {
            // add swagger document for every API version discovered
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    CreateVersionInfo(description));
            }

            options.ExampleFilters();

            // For adding xml commenting (see also Documentation file in project properties)
            // https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#generatedocumentationfile
            var xmlCommentsPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            options.IncludeXmlComments(xmlCommentsPath);

            options.MapType(typeof(TimeSpan?), () => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00:00")
            });

            // Because capable to use AutoREST we must to disable UseAllOfToExtendReferenceSchemas
            // for more information please see https://stackoverflow.com/q/59788412/1539100
            // and https://github.com/unchase/Unchase.Swashbuckle.AspNetCore.Extensions/issues/13
            // options.UseAllOfToExtendReferenceSchemas();

            // operationId is an optional unique string used to identify an operation.
            // If provided, these IDs must be unique among all operations described in your API.
            //
            // However, AutoRest seems to use that to identify each method.
            // I found a GitHub question / issue: <see href:https://github.com/Azure/autorest/issues/2647/>
            // where people addressed this by configuring AutoRest to use tags instead of operation ID to identify method.
            //
            // <see href:https://stackoverflow.com/a/60875558/1539100/>
            options.CustomOperationIds(description => (description.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
        }

        /// <summary>
        /// Configure Swagger Options. Inherited from the Interface
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        /// <summary>
        /// Create information about the version of the API
        /// </summary>
        /// <param name="desc"></param>
        /// <returns>Information about the API</returns>
        private OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
        {
            var info = new OpenApiInfo()
            {
                Title = "Web MVC Test Project",
                Version = desc.ApiVersion.ToString(),
                Description = "Web MVC Test Project with Autofac and Swagger",
                TermsOfService = new Uri("https://resaa.net"),
                License = new OpenApiLicense
                {
                    Name = "BSN Corporation",
                    Url = new Uri("https://resaa.net/LICENSE"),
                }

            };

            if (desc.IsDeprecated)
            {
                info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
            }

            return info;
        }

        private readonly IApiVersionDescriptionProvider _provider;
    }
}
