using Api.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/docs.json");
}

var clientWwwRoot = Path.Combine(builder.Environment.ContentRootPath, "..", "Client", "wwwroot");
// app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(clientWwwRoot),
    RequestPath = ""
});

// //Servir imagenes del servidor
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
