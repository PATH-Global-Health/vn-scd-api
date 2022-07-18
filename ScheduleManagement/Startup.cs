using AutoMapper;
using Data.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScheduleManagement.Extensions;
using Serilog;
using Services.LookupServices;
using System;

namespace ScheduleManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigCors();
            services.ConfigValidationProblem();
            services.AddControllers();
            services.AddDbContexts(Configuration);
            services.ConfigJwt(Configuration);
            services.ConfigSwagger(Env);
            if (!Env.IsDevelopment() && !(Configuration["DevName"] == "Duong"))
                services.ConfigSerilog(Configuration);
            services.BusinessServicesDI(Configuration, Env);
            services.AddAutoMapper();
            services.AddElasticsearch(Configuration);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["Redis:Connection"];
                options.InstanceName = Configuration["Redis:Instance"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Middleware call order: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1#middleware-order
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, AppDbContext appDbContext, 
                            IndicatorLookupService lookup,
                            PackageLookupService packageLookup)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Console.WriteLine("Run");

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseOpenApi();
            app.UseSwaggerUi3();
            loggerFactory.AddSerilog();

            app.UseMapsterConfig(lookup, packageLookup);
        }
    }
}
