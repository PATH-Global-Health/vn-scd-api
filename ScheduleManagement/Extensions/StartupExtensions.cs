using System;
using System.Linq;
using Data.DbContexts;
using Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Service.RabbitMQ;
using Services;
using Services.LookupServices;
using Services.RabbitMQ;
using Services.SMDServices;
using Services.Utilities;

namespace ScheduleManagement.Extensions
{
    public static class StartupExtensions
    {
        public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {

            if (ExternalEnv.APP_CONNECTION_STRING != null)
            {
                services.AddDbContext<AppDbContext>
                (options =>
                    options.UseLazyLoadingProxies()
                        .UseSqlServer(ExternalEnv.APP_CONNECTION_STRING));
            }
            else
            {
                services.AddDbContext<AppDbContext>
                (options =>
                    options.UseLazyLoadingProxies()
                        .UseSqlServer(configuration["ConnectionStrings:DbConnection"]));
            }

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void BusinessServicesDI(this IServiceCollection services, IConfiguration Configuration, IWebHostEnvironment env)
        {

            if (!env.IsDevelopment() && !(Configuration["DevName"] == "Duong"))
            {
                services.AddSingleton<IHostedService, Consumer>();
                services.AddSingleton<IHostedService, ConsumerCheckExternalId>();
                services.AddSingleton<IHostedService, ConsumerReferTicket>();
                services.AddSingleton<IHostedService, ConsumerSetStatusProfile>();
                services.AddSingleton<IHostedService, ConsumerConfirmedCustomer>();
            }
                
            services.AddSingleton<IProducer, Producer>();
            services.AddSingleton<IProducerCheckConfirmedCustomer, ProducerCheckConfirmedCustomer>();
            services.AddSingleton<IProducerDeleteUser, ProducerDeleteUser>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IWorkingCalendarService, WorkingCalendarService>();
            services.AddScoped<IDayService, DayService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IIntervalService, IntervalService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IUtilService, UtilService>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<IServiceFormService, ServiceFormService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IUnitTypeService, UnitTypeService>();
            services.AddScoped<IUnitDoctorService, UnitDoctorService>();
            services.AddScoped<IServiceUnitService, ServiceUnitService>();
            services.AddScoped<IInjectionObjectService, InjectionObjectService>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddScoped<IInjectionObjectServiceTypeService, InjectionObjectServiceTypeService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<ILayTestService, LayTestService>();

            services.AddScoped<IReferTicketService, ReferTicketService>();
            services.AddScoped<IProfileLinkService, ProfileLinkService>();

            services.AddScoped<IitemService, ItemService>();
            services.AddScoped<IitemSourceService, ItemSourceService>();
            services.AddSingleton<ICacheService, CacheService>();


            #region SMD
            services.AddScoped<IIndicatorService, IndicatorService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IPatientInfoService, PatientInfoService>();
            services.AddScoped<IUtilitiesService, UtilitiesService>();

            services.AddSingleton<IndicatorLookupService>();
            services.AddSingleton<PackageLookupService>();
            services.AddSingleton<SMDUserLookupService>();
            #endregion
        }

        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", builder =>
                                    builder.AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin()
                                            //.WithOrigins(new[] { "http://localhost:3000",
                                            //    "http://testdichvu.hcdc.vn",
                                            //    "http://202.78.227.99:31290",
                                            //    "http://localhost:3001",
                                            //    "http://202.78.227.94:30666",
                                            //    "https://hcdc.vn",
                                            //    "http://dichvu.hcdc.vn",
                                            //    "https://dichvu.hcdc.vn",
                                            //    "http://tiemchung.hcdc.vn",
                                            //    "http://hcdc.vn",
                                            //    "http://xetnghiem.hcdc.vn",
                                            //    "http://localhost:8100",
                                            //    "http://202.78.227.176:30082"
                                            //})
                                            )

            );
        }

        public static void ConfigSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticUri = configuration["ElasticConfiguration:Uri"];
            var elasticUsername = configuration["ElasticConfiguration:Username"];
            var elasticPassword = configuration["ElasticConfiguration:Password"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                    ModifyConnectionSettings = x => x.BasicAuthentication(elasticUsername, elasticPassword),
                })
            .CreateLogger();
        }

        public static void ConfigValidationProblem(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(new
                    {
                        StatusCode = 400,
                        Message = context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage),
                        Error = "Bad request"
                    });

                };
            });
            
        }
    }
}
