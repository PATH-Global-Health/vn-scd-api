using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleManagement.Extensions
{
    public static class JwtExtensions
    {
        public static void ConfigJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSecretKey"]));

            services
                .AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtconfig =>
                {
                    jwtconfig.SaveToken = true;
                    jwtconfig.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JwtIssuerOptions:Issuer"],
                        ValidAudience = configuration["JwtIssuerOptions:Issuer"],
                        IssuerSigningKey = key
                    };

                    jwtconfig.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                           
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            
                            return Task.CompletedTask;
                        },
                    };
                });
        }
    }
}
