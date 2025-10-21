using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string appUri = builder.HostEnvironment.BaseAddress;
string identityUri = "http://localhost:5253";


var cookieContainer = new CookieContainer();

builder.Services.AddSingleton(cookieContainer);

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthService>());

builder.Services.AddScoped(provider => new JwtTokenMessageHandler(provider.GetRequiredService<AuthService>()));

builder.Services.AddHttpClient("adoptions", client =>
    {
        client.BaseAddress = new Uri(appUri);   
    })
    .AddHttpMessageHandler<JwtTokenMessageHandler>();

builder.Services.AddHttpClient("identity", client => client.BaseAddress = new Uri(identityUri))
    .AddHttpMessageHandler<JwtTokenMessageHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AnimalService>();


var app = builder.Build();
await RefreshJwtToken(app);

await app.RunAsync();

async Task RefreshJwtToken(WebAssemblyHost application)
{
    using var boostrapScope = application.Services.CreateScope();
    var api = boostrapScope.ServiceProvider.GetRequiredService<AuthService>();
    var result = await api.RefreshTokenAsync();
    if (result.Success)
    {
        _ = api.ScheduleTokenRefresh();
        Console.WriteLine(result.Message);
    }
    else
    {
        Console.WriteLine(result.Message);
    }
}


