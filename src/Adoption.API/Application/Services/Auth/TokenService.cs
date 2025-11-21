using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Adoption.API.Utils.Options;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;
namespace Adoption.Infrastructure.Services;

public class TokenService
{
    private const string RefreshTokenName = "RefreshToken";
    private const string RefreshTokenProvider = "RefreshTokenProvider";
    private readonly IOptions<JwtOptions> _options;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AdoptionDbContext _context;

    public TokenService(IOptions<JwtOptions> options, UserManager<ApplicationUser> userManager, AdoptionDbContext context)
    {
        _options = options;
        _userManager = userManager;
        _context = context;
    }

    public async Task<string> GenerateAccessToken(ApplicationUser user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.Value.SigningKey));

        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach(var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _options.Value.Url,
            audience: _options.Value.Audience, 
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToDouble(_options.Value.ExpireMinutes)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshToken(ApplicationUser user)
    {
        var tokenString = Guid.NewGuid().ToString("N");

        var refreshToken = new TokenData()
        {
            Token = tokenString,
            Expiration = DateTime.UtcNow.AddDays(7)
        };

        var json = JsonSerializer.Serialize(refreshToken);

        await _userManager.RemoveAuthenticationTokenAsync(
            user,
            RefreshTokenProvider,
            RefreshTokenName);

        await _userManager.SetAuthenticationTokenAsync(
            user,
            RefreshTokenProvider,
            RefreshTokenName,
            json);

        return tokenString;
    }

    public async Task<Token> GenerateTokenDto(ApplicationUser appUser)
    {
        var accessToken = await GenerateAccessToken(appUser);
        var refreshToken = await GenerateRefreshToken(appUser);

        return new Token
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = Convert.ToInt32(_options.Value.ExpireMinutes)
        };
    }


    public async Task<ApplicationUser?> FindUserByRefreshToken(string refreshToken)
    {
        var tokenEntries = await _context.UserTokens
            .Where(t =>
                t.LoginProvider == RefreshTokenProvider &&
                t.Name == RefreshTokenName)
            .ToListAsync();

        foreach (var tokenEntry in tokenEntries)
        {
            var tokenData = JsonSerializer.Deserialize<TokenData>(tokenEntry.Value);
            if (tokenData?.Token == refreshToken)
            {
                return await _userManager.FindByIdAsync(tokenEntry.UserId);
            }
        }

        return null;
    }

    public async Task<Result<TokenData>> ValidateRefreshToken(string refreshToken, ApplicationUser appUser)
    {
        //Obtenemos el token del usuario
        var currentToken = await _userManager.GetAuthenticationTokenAsync(appUser, RefreshTokenProvider, RefreshTokenName);
        var tokenData = JsonSerializer.Deserialize<TokenData>(currentToken);

        if (tokenData is null || tokenData.Token != refreshToken)
            return Result<TokenData>.FromFailure("Token mismatch");

        if (tokenData.Expiration < DateTime.UtcNow)
            return Result<TokenData>.FromFailure("Token expired");

        return Result<TokenData>.FromData(tokenData);
    }

}

public class TokenData
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
