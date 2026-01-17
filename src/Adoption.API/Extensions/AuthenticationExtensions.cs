using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Adoption.API.Extensions;

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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminOnlyPolicy", policy =>
            {
                policy.RequireRole("SuperAdmin");
            });

            options.AddPolicy("AnimalsManagementPolicy", policy =>
            {
                policy.RequireRole("SuperAdmin", "Owner", "Admin");
            });
            
            options.AddPolicy("UsersManagementPolicy", policy =>
            {
                policy.RequireRole("SuperAdmin", "Admin");
            });

            options.AddPolicy("RequesterPolicy", policy =>
            {
                policy.RequireRole("SuperAdmin", "Requester", "Admin");
            });
            
            options.AddPolicy("OwnerPolicy", policy =>
            {
                policy.RequireRole("SuperAdmin", "Admin", "Owner");
            });
            
            options.AddPolicy("OwnerRequesterPolicy", policy =>
            {
                policy.RequireRole("Requester", "Owner");
            });

        });

        return services;
    }
    
    public static Guid? GetUserIdFromContext(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId) ? userId : Guid.Empty;
    }
}
