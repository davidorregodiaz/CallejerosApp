using Adoption.API;
using Adoption.API.Application.Services.DbSeeder;
using Adoption.API.Endpoints;
using Adoption.API.Extensions;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplication();
builder.AddIdentity();
builder.AddDefaultAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
}
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Adoption API v1");
    options.RoutePrefix = "";
});

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder<AdoptionDbContext>>();
    var context = scope.ServiceProvider.GetRequiredService<AdoptionDbContext>();
    context.Database.Migrate();
    await seeder.SeedAsync(context);
}

app.UseCors("AllowReactApp");

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("/api")
    .MapAnimalEndpoints()
    .MapAdoptionEndpoints()
    .MapAppointmentEndpoints()
    .MapUserEndpoints();

app.MapGet("/hello", () => "Hello World!");

await app.RunAsync("http://0.0.0.0:5239");
