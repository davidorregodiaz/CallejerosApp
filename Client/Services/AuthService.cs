using System;
using System.Net.Http.Json;
using System.Security.Claims;
using Client.Models;
using Client.Utilites;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Shared;

namespace Client.Services;

public class AuthService(IJSRuntime js,IHttpClientFactory clientFactory) : AuthenticationStateProvider
{
    private readonly ClaimsIdentity _anonymous = new();
    private const string TokenKey = "TokenKey";

    public async Task<string?> GetToken() => await js.GetFromLocalStorage(TokenKey);


    public async Task<TaskResult> SignIn(UserLoginModel model)
    {
        try
        {
            var client = clientFactory.CreateClient("Api");
            var response = await client.PostAsJsonAsync("api/auth/login", model);
            response.EnsureSuccessStatusCode();
            var tokenModel = await response.Content.ReadFromJsonAsync<TokenModel>();
            Console.WriteLine(tokenModel);
            await js.SetToLocalStorage(TokenKey, tokenModel.Token);
            var principal = JwtSerialize.Deserialize(tokenModel.Token);
            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            return new TaskResult { Success = true, Message = "Login successful!" };
        }
        catch (Exception ex)
        {
            return new TaskResult { Success = false, Message = ex.Message };
        }
        
    }

    public async Task<TaskResult> SignOut()
    {
        await js.RemoveFromLocalStorage(TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(_anonymous))));
        return new TaskResult { Success = true, Message = "Logout successful!" };
    }

    public async Task<TaskResult> Register(UserRegisterModel model)
    {
        // Simulate an API call for registration
        await Task.Delay(1000); // Simulating network delay

        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return new TaskResult { Success = false, Message = "Email and Password are required." };
        }

        // Here you would typically call your API to register the user
        // For now, we will just simulate a successful registration
        return new TaskResult { Success = true, Message = "Registration successful!" };
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await js.GetFromLocalStorage(TokenKey);

        if (token is not null)
        {
            var principal = JwtSerialize.Deserialize(token);
            return new AuthenticationState(principal);   
        }

        return new AuthenticationState(new ClaimsPrincipal(_anonymous));
    }
    
    private record TokenModel(string Token, int Expiration);
}
