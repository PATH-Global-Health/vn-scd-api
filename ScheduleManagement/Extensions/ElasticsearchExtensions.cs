using Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nest;
using System;
using System.Text;
using System.Threading.Tasks;


namespace ScheduleManagement.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];
            var uri = configuration["ElasticConfiguration:Uri"];
            var username = configuration["ElasticConfiguration:Username"];
            var password = configuration["ElasticConfiguration:Password"];

            var settings = new ConnectionSettings(new Uri(uri))
                .DefaultIndex(defaultIndex)
                //.BasicAuthentication(username, password)
                .ServerCertificateValidationCallback((o, certificate, arg3, arg4) => { return true; }).BasicAuthentication(username, password)
                .DefaultMappingFor<WorkingCalendar>(m => m
                    .PropertyName(p => p.Id, "id")
                )
                .DefaultMappingFor<Data.Entities.Day>(m => m
                    .PropertyName(c => c.Id, "id")
                )
                .DefaultMappingFor<Data.Entities.ServiceWorkingCalendar>(m => m
                    .PropertyName(c => c.Id, "id")
                    )
                .DefaultMappingFor<Data.Entities.RoomWorkingCalendar>(m => m
                    .PropertyName(c => c.Id, "id")
                    )
                .DefaultMappingFor<Data.Entities.DoctorWorkingCalendar>(m => m
                    .PropertyName(c => c.Id, "id"));

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
