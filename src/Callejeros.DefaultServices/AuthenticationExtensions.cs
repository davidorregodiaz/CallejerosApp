using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Callejeros.DefaultServices;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var jwtOptions = configuration.GetSection("Jwt");
                     
        if (!jwtOptions.Exists())
        {
            Console.WriteLine("Doesn't exist this section");
            return services;
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = jwtOptions.GetValue<string>("Url");
            options.Audience = jwtOptions.GetValue<string>("Audience");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.GetValue<string>("Url"),
                ValidateAudience = true,
                ValidAudience = jwtOptions.GetValue<string>("Audience"),
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.GetValue<string>("SigningKey")!))
            };
            options.RequireHttpsMetadata = false;
        });

        services.AddAuthorization();

        return services;
    }
}
