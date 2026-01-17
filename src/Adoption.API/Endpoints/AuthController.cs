using System.Security.Claims;
using Adoption.API.Application.Models.User;
using Adoption.API.Application.Services.Auth;
using Identity.API.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adoption.API.Endpoints;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<TokenViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("User login into the system")]
    public async Task<IResult> Login([FromBody] LoginViewModel userLoginDto)
    {
        var result = await authService.Login(userLoginDto);
        if (result.IsSuccessful(out var token))
        {
            SetRefreshTokenCookie(token.RefreshToken);
            return Results.Ok(new TokenViewModel
            {
                Token = token.AccessToken,
                ExpiresIn = token.ExpiresIn,
                User = token.User,
                RefreshToken = token.RefreshToken
            });
        }

        return Results.BadRequest(new { error = result.Message });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<TokenViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("User register in the system")]
    public async Task<IResult> Register([FromForm] RegisterViewModel registerUserDto)
    {
        var result = await authService.Register(registerUserDto);
        if (result.IsSuccessful(out var token))
        {
            SetRefreshTokenCookie(token.RefreshToken);
            return Results.Ok(new TokenViewModel
            {
                Token = token.AccessToken,
                ExpiresIn = token.ExpiresIn,
                User = token.User,
                RefreshToken = token.RefreshToken
            });
        }

        return Results.BadRequest(new { error = result.Message });
    }

    public record RefreshTokenRequest(string RefreshToken);

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType<TokenViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("Refresh the access token and refresh token")]
    public async Task<IResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        
        if (string.IsNullOrEmpty(refreshToken))
            return Results.BadRequest("Refresh token missing");

        var result = await authService.RefreshToken(refreshToken);

        if (!result.IsSuccessful(out var tokenDto))
            return Results.Unauthorized();
        
        SetRefreshTokenCookie(tokenDto.RefreshToken);
        
        return Results.Ok(new
        {
            AccessToken = tokenDto.AccessToken,
            RefreshToken = tokenDto.RefreshToken,
            ExpiresIn = tokenDto.ExpiresIn,
            User = tokenDto.User
        });
    }

    [AllowAnonymous]
    [HttpGet("check")]
    [ProducesResponseType<TokenViewModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Checks if the user is logged in")]
    public IActionResult Check()
    {
        return User.Identity!.IsAuthenticated ? Ok() : Forbid();
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Logout the user from the system")]
    public void LogOut()
    {
        Response.Cookies.Delete("refresh_token");
    }


    [HttpGet("/claims")]
    [EndpointSummary("Get the user claims")]
    public void ChekUserClaims()
    {
        var roles = HttpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

        logger.LogInformation(string.Join(",", roles));
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
