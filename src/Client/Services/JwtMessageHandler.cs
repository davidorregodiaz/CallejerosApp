using System;
using System.Net.Http.Headers;
using System.Text.Json;
using Client.Models;
using Client.Utilites;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Client.Services;

public class JwtTokenMessageHandler : DelegatingHandler
{
    private readonly Uri _allowedBaseAddress;
    private readonly AuthService _authService;

    public JwtTokenMessageHandler(Uri allowedBaseAddress, AuthService authService)
    {
        _allowedBaseAddress = allowedBaseAddress;
        _authService = authService;
    }


    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return this.SendAsync(request, cancellationToken).Result;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri;
        var isSelfApiAccess = this._allowedBaseAddress.IsBaseOf(uri);

        var tokenString = await _authService.GetToken();
        
        if (!string.IsNullOrEmpty(tokenString))
        {
            var tokenModel = JsonSerializer.Deserialize<TokenModel>(tokenString);

            if (isSelfApiAccess)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenModel?.Token);
            }
        }
        

        return await base.SendAsync(request, cancellationToken);
    }
}
