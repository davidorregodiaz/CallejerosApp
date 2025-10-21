using Adoption.API.Abstractions;
using Adoption.API.Application.Behaviors;
using Adoption.API.Application.Queries;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Adoption.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API;

public static class DependencyInjection
{
    public static void AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer(); // escanea endpoints
        builder.Services.AddSwaggerGen();  
        builder.Services.AddRazorPages(); // Necesario para Blazor
        builder.Services.AddDbContext<AdoptionDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddScoped<IAnimalQueries, AnimalQueries>();

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
        
        builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(TransactionDecorator.CommandHandler<,>));
        builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(ValidatorDecorator.CommandHandler<,>));
        builder.Services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
    }
}
