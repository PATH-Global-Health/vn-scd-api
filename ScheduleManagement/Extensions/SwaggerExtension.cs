using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Linq;
using Newtonsoft.Json.Converters;

namespace ScheduleManagement.Extensions
{
    public static class SwaggerExtension
    {
        public static void ConfigSwagger(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddOpenApiDocument(document =>
            {
                
                document.Title = "Schedule Management";
                if (env.IsDevelopment())
                    document.Title += " Dev";
                if (env.IsStaging())
                    document.Title = " Staging";
                if (env.IsProduction())
                    document.Title = " Production";
                document.Version = "2.0.1";
                document.AllowReferencesWithProperties = true;
                document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                //      new OperationSecurityScopeProcessor("JWT"));
            });
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()));
        }
    }
}
