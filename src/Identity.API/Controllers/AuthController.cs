using Identity.API.Models.ViewModels;
using Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginViewModel userLoginDto)
    {
        var result = await _authService.Login(userLoginDto);
        if (result.IsSuccessful(out var token))
        {
            Response.Cookies.Append("refresh_token", token.RefreshToken);
            return Results.Ok(new TokenViewModel
            {
                Token = token.AccessToken,
                ExpirationMinutes = token.ExpiresIn
            });
        }
        
        return Results.BadRequest(new { error = result.Message });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterViewModel registerUserDto)
    {
        var result = await _authService.Register(registerUserDto);
        if (result.IsSuccessful(out var token))
        {
            Response.Cookies.Append("refresh_token", token.RefreshToken);
            return Results.Ok(new TokenViewModel
            {
                Token = token.AccessToken,
                ExpirationMinutes = token.ExpiresIn
            });
        }
        return Results.BadRequest(new { error = result.Message });
    }

    [AllowAnonymous]
    [HttpGet("refresh-token")]
    public async Task<IResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken))
            return Results.BadRequest("Refresh token missing");

        var result = await _authService.RefreshToken(refreshToken);

        if (!result.IsSuccessful(out var tokenDto))
            return Results.BadRequest(result.Message);

        SetRefreshTokenCookie(tokenDto.RefreshToken); 
        
        return Results.Ok(new TokenViewModel
        {
            Token = tokenDto.AccessToken,
            ExpirationMinutes = tokenDto.ExpiresIn
        });
    }

    [AllowAnonymous]
    [HttpGet("check")]
    public IActionResult Check()
    {
        return User.Identity!.IsAuthenticated ? Ok() : Unauthorized();
    }

    [Authorize]
    [HttpGet("logOut")]
    public void LogOut()
    {
        Response.Cookies.Delete("refresh_token");
    }
    
    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Secure=true en producción, false en desarrollo
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        };

        Response.Cookies.Append("refresh_token", token, cookieOptions);
    }
}
