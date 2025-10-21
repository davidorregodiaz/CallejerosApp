using Adoption.API;
using Callejeros.DefaultServices;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var clientWwwRoot = Path.Combine(builder.Environment.ContentRootPath, "..", "Client", "wwwroot");

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
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();


// Imagenes del cliente 
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(clientWwwRoot),
    RequestPath = ""
});

// Imagenes del servidor subidas por los usuarios
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "images/upload")),
    RequestPath = "/images/upload"
});

app.UseBlazorFrameworkFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
