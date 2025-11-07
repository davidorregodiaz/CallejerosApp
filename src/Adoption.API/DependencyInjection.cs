using Adoption.API.Abstractions;
using Adoption.API.Application.Behaviors;
using Adoption.API.Application.Queries;
using Adoption.API.Application.Services;
using Adoption.API.Middlewares;
using Adoption.API.Utils.Options;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Adoption.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Minio;

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
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AdoptionDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Configuration.AddJsonFile("animalCatalog.json", optional: false, reloadOnChange: true);
        builder.Services.Configure<AnimalOptions>(
            builder.Configuration.GetSection("AnimalCatalog")
        );

        builder.Services.Configure<MinioOptions>(
            builder.Configuration.GetSection("Minio")
        );
        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddScoped<IMinioService, MinioService>();

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
    }
}
