using System.Net.Http.Headers;
using System.Text.Json;
using Client.Models;

namespace Client.Services;

public class JwtTokenMessageHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public JwtTokenMessageHandler( AuthService authService)
    {
        _authService = authService;
    }


    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return this.SendAsync(request, cancellationToken).Result;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri;

        var tokenString = await _authService.GetToken();
        
        if (!string.IsNullOrEmpty(tokenString))
        {
            var tokenModel = JsonSerializer.Deserialize<TokenModel>(tokenString);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenModel?.Token);
            request.Headers.Add("refresh_token", tokenModel?.RefreshToken);
        }
        

        return await base.SendAsync(request, cancellationToken);
    }
}
