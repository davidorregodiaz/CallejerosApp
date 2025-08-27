using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Dtos;
using Shared.Utilities;

namespace Identity.API.Services;

public class TokenService
{
    private const string RefreshTokenName = "RefreshToken";
    private const string RefreshTokenProvider = "RefreshTokenProvider";
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _configuration = configuration;
        _userManager = userManager;
        _context = context;
    }

    public string GenerateAccessToken(ApplicationUser user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]!));
        
        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id), 
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
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

    public async Task<TokenModel> GenerateTokenDto(ApplicationUser appUser)
    {
        var accessToken = GenerateAccessToken(appUser);
        var refreshToken = await GenerateRefreshToken(appUser);

        return new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])
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

    public async Task<TaskResult<TokenData>> ValidateRefreshToken(string refreshToken,ApplicationUser appUser)
    {
        //Obtenemos el token del usuario
        var currentToken = await _userManager.GetAuthenticationTokenAsync(appUser, RefreshTokenProvider, RefreshTokenName);
        var tokenData = JsonSerializer.Deserialize<TokenData>(currentToken);
        
        if (tokenData is null || tokenData.Token != refreshToken)
            return TaskResult<TokenData>.FromFailure("Token mismatch");
        
        if (tokenData.Expiration < DateTime.UtcNow)
            return TaskResult<TokenData>.FromFailure("Token expired");

        return TaskResult<TokenData>.FromData(tokenData);
    }

}
