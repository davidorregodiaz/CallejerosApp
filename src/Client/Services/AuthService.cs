using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Client.Models;
using Client.Utilites;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Shared;

namespace Client.Services;

public class AuthService(IJSRuntime js, IHttpClientFactory clientFactory) : AuthenticationStateProvider
{
    private readonly ClaimsIdentity _anonymous = new();
    private const string TokenKey = "TokenKey";
    private Timer? _refreshTimer;
    private const int _refreshMarginMinutes = 5;

    public async Task<string?> GetToken() => await js.GetFromLocalStorage(TokenKey);


    public async Task<Result> SignIn(UserLoginModel model)
    {
        try
        {
            var client = clientFactory.CreateClient("identity");
            var response = await client.PostAsJsonAsync("api/auth/login", model);
            response.EnsureSuccessStatusCode();
            var tokenModel = await response.Content.ReadFromJsonAsync<TokenModel>();
            await js.SetToLocalStorage(TokenKey, JsonSerializer.Serialize(tokenModel));
            var principal = JwtClaimsParser.Deserialize(tokenModel.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            _ = ScheduleTokenRefresh();
            return new Result { Success = true, Message = "[SUCC] Login successful!" };
        }
        catch (Exception ex)
        {
            return new Result { Success = false, Message = "[ERR] " + ex.Message };
        }

    }

    public async Task ScheduleTokenRefresh()
    {
        var tokenString = await GetToken();
        var tokenModel = JsonSerializer.Deserialize<TokenModel>(tokenString);
        var refreshTime = TimeSpan.FromMinutes(tokenModel.ExpirationMinutes - _refreshMarginMinutes);

        _refreshTimer = new Timer(async _ => await RefreshTokenAsync(),
            null,
            refreshTime,
            Timeout.InfiniteTimeSpan);
    }
    public async Task<Result> RefreshTokenAsync()
    {
        try
        {
            var client = clientFactory.CreateClient("identity");
            var response = await client.GetAsync("api/auth/refresh-token");

            if (response.IsSuccessStatusCode)
            {
                var tokenModel = await response.Content.ReadFromJsonAsync<TokenModel>();
                await js.SetToLocalStorage(TokenKey, JsonSerializer.Serialize(tokenModel));
                return Result.FromSuccess("[SUCC] Token Reactivated");
            }
            else
            {
                var tokenString = await GetToken();
                if (!string.IsNullOrEmpty(tokenString))
                {
                    await js.RemoveFromLocalStorage(TokenKey);
                    _refreshTimer?.Dispose();
                    return Result.FromFailure("[WARN] Token expired");
                }
                return Result.FromFailure("[WARN] User must be authenticated");
            }
        }
        catch (Exception ex)
        {
            return Result.FromFailure("[ERR] Exception Caught " + ex.Message);
        }
    }

    public async Task SignOut()
    {
        var client = clientFactory.CreateClient("identity");
        // await client.GetAsync("api/auth/logOut");
        await js.RemoveFromLocalStorage(TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(_anonymous))));
        Console.WriteLine("Signed out");
    }

    // public async Task<Result> ChangePassword(UserRegisterModel model)
    // {
    //     if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
    //     {
    //         return new Result { Success = false, Message = "Old and New Passwords are required." };
    //     }

    //     var client = clientFactory.CreateClient("Api");
    //     var response = await client.PostAsJsonAsync("api/auth/change-password", model);
    //     if (response.IsSuccessStatusCode)
    //     {
    //         return new Result { Success = true, Message = "Password changed successfully!" };
    //     }
    //     else
    //     {
    //         return new Result { Success = false, Message = "Failed to change password." };
    //     }
    // }   

    public async Task<Result> SignUp(UserRegisterModel model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return new Result { Success = false, Message = "Email and Password are required." };
        }

        var client = clientFactory.CreateClient("identity");
        var response = await client.PostAsJsonAsync("api/auth/register", model);
        if (response.IsSuccessStatusCode)
        {
            return new Result { Success = true, Message = "Registration successful!" };
        }
        else
        {
            return new Result { Success = false, Message = "Failed to register." };
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetToken();

        if (token is not null)
        {
            var principal = JwtClaimsParser.Deserialize(token);
            return new AuthenticationState(principal);
        }

        return new AuthenticationState(new ClaimsPrincipal(_anonymous));
    }


}
