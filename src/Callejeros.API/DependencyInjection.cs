using Adoption.API.Abstractions;
using Adoption.API.Application.Behaviors;
using Adoption.API.Application.Jobs;
using Adoption.API.Application.Queues;
using Adoption.API.Application.Services.Auth;
using Adoption.API.Application.Services.DbSeeder;
using Adoption.API.Application.Services.Email;
using Adoption.API.Application.Services.Mappers;
using Adoption.API.Application.Services.Minio;
using Adoption.API.Middlewares;
using Adoption.API.Utils.Options;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Domain.Events;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using Adoption.Infrastructure.Repositories;
using Adoption.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Quartz;


namespace Adoption.API;
public static class DependencyInjection
{
    public static void AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Callejeros app Api", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddDbContext<AdoptionDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Configuration.AddJsonFile("animalCatalog.json", optional: false, reloadOnChange: true);

        builder.Services.Configure<AnimalOptions>(
            builder.Configuration.GetSection("AnimalCatalog")
        );

        builder.Services.Configure<EmailOptions>(
            builder.Configuration.GetSection("Email"));

        builder.Services.Configure<MinioOptions>(
            builder.Configuration.GetSection("Minio")
        );

        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("Jwt")
        );

        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddScoped<IAdoptionRequestRepository, AdoptionRequestsRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IMinioService, MinioService>();
        builder.Services.AddScoped<IAnimalMapper, AnimalMapper>();
        builder.Services.AddScoped<IAdoptionMapper, AdoptionMapper>();

        builder.Services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(TransactionDecorator.QueryHandler<,>));
        builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(TransactionDecorator.CommandHandler<,>));
        builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(ValidatorDecorator.CommandHandler<,>));
        builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));

        builder.Services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        builder.Services.AddScoped<IDbSeeder<AdoptionDbContext>, DbSeeder>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        builder.Services.AddSingleton<IEmailQueue, EmailQueue>();
        
        builder.Services.AddQuartz(config =>
        {
            var jobKey = new JobKey("ProcessEmailJob");

            config.AddJob<ProcessEmailJob>(opts => opts.WithIdentity(jobKey));

            config.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ProcessEmailJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(30) 
                    .RepeatForever()
                )
            );
        });
        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);
        
        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        builder.Services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        builder.Services.Decorate(typeof(IDomainEventHandler<>),  typeof(LoggingDecorator.DomainEventHandler<>));
        
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000") 
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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
            .AddEntityFrameworkStores<AdoptionDbContext>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<TokenService>();
    }
}
