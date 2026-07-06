using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopSphere.Infrastructure.Authentication;

public sealed class JwtTokenProvider(
    IOptions<JwtOptions> options)
    : ITokenProvider
{
    private readonly JwtOptions _options = options.Value;

    public Task<TokenResponse> CreateAsync(
        string userId,
        string email,
        IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            _options.ExpirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Task.FromResult(
            new TokenResponse(accessToken, expires));
    }
}