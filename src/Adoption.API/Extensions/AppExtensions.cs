using System.Text;
using Adoption.API.Application.Queries;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Adoption.Infrastructure.Repositories;
// using Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Adoption.API.Extensions;

public static class AppExtensions
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddRazorPages(); // Necesario para Blazor
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<AdoptionDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorClient",
                builder => builder
                    .WithOrigins("https://localhost:7074", "http://localhost:5194") // URLs de tu cliente Blazor
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        // //Identity
        // builder.Services.AddIdentityCore<AppUser>(options =>
        //     {
        //         options.SignIn.RequireConfirmedAccount = false;
        //         options.Password.RequireDigit = true;
        //         options.Password.RequireLowercase = true;
        //         options.Password.RequireUppercase = true;
        //         options.Password.RequiredLength = 8;
        //         options.User.RequireUniqueEmail = true;
        //         options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //         options.Lockout.MaxFailedAccessAttempts = 5;
        //     })
        //     .AddEntityFrameworkStores<AdoptionDbContext>();

        //Authentication JWT
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
                };
            });

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.SameAsRequest;
        });

        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddScoped<IAnimalQueries, AnimalQueries>();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    }
}
