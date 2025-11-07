

using Callejeros.DefaultServices;
using Identity.API;
using Identity.API.Data;
using Identity.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

builder.AddIdentity();
builder.AddDefaultAuthentication();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();

app.UseCors("AllowBlazorClient");

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();

