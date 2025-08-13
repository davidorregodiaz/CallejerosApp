using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var appUri = builder.HostEnvironment.BaseAddress;

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthService>());

builder.Services.AddScoped(provider => new JwtTokenMessageHandler(new Uri(appUri), provider.GetRequiredService<AuthService>()));

builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(appUri))
    .AddHttpMessageHandler<JwtTokenMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AnimalService>();


await builder.Build().RunAsync();

