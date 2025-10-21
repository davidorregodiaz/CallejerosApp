using System.Text;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API;

public static class Extensions
{
    // Identity
    public static IServiceCollection AddIdentity(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorClient",
                builder => builder
                    .WithOrigins("https://localhost:7097", "http://localhost:5194","http://localhost:5239") // URLs de tu cliente Blazor
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        return services;
    }
        
}
