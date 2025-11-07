using Adoption.API;
using Adoption.API.Endpoints;
using Callejeros.DefaultServices;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplication();
builder.AddDefaultAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Adoption API v1");
        options.RoutePrefix = "";
    });
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api")
    .MapAnimalEndpoints();

app.MapGet("/hello", () => "Hello World!");

app.Run();
