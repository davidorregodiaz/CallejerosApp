using Adoption.API.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var clientWwwRoot = Path.Combine(builder.Environment.ContentRootPath, "..", "Client", "wwwroot");

builder.AddAppServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/docs.json");
}


app.UseCors("AllowBlazorClient");

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

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
    RequestPath = "/wwwroot/images/upload"
});

app.UseBlazorFrameworkFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
