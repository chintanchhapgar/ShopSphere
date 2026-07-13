using FluentAssertions;
using Microsoft.Extensions.Options;
using ShopSphere.Contracts.Authentication;
using ShopSphere.Infrastructure.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopSphere.InfrastructureTests.Authentication;

public sealed class JwtTokenProviderTests
{
    private readonly JwtOptions _options;
    private readonly JwtTokenProvider _provider;

    public JwtTokenProviderTests()
    {
        _options = new JwtOptions
        {
            Issuer = "ShopSphere",
            Audience = "ShopSphere.Api",
            SecretKey = "ThisIsAVeryLongSecretKeyForJwtTesting123456789",
            ExpirationInMinutes = 60
        };

        _provider = new JwtTokenProvider(
            Options.Create(_options));
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Token_Response()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await _provider.CreateAsync(
            userId,
            "john@test.com",
            ["Customer"]);

        // Assert
        result.Should().NotBeNull();

        result.AccessToken.Should().NotBeNullOrWhiteSpace();

        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Valid_Jwt()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await _provider.CreateAsync(
            userId,
            "john@test.com",
            ["Customer"]);

        // Assert
        var handler = new JwtSecurityTokenHandler();

        handler.CanReadToken(result.AccessToken)
            .Should().BeTrue();

        var jwt = handler.ReadJwtToken(result.AccessToken);

        jwt.Issuer.Should().Be(_options.Issuer);

        jwt.Audiences.Should().Contain(_options.Audience);
    }

    [Fact]
    public async Task CreateAsync_Should_Contain_Sub_Email_And_Jti_Claims()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await _provider.CreateAsync(
            userId,
            "john@test.com",
            ["Customer"]);

        var jwt = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.AccessToken);

        // Assert
        jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub)
            .Value.Should().Be(userId);

        jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email)
            .Value.Should().Be("john@test.com");

        jwt.Claims.Should()
            .Contain(x => x.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public async Task CreateAsync_Should_Contain_All_Role_Claims()
    {
        // Arrange
        var roles = new[] { "Admin", "Vendor", "Customer" };

        // Act
        var result = await _provider.CreateAsync(
            Guid.NewGuid().ToString(),
            "john@test.com",
            roles);

        var jwt = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.AccessToken);

        // Assert
        var roleClaims = jwt.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();

        roleClaims.Should().BeEquivalentTo(roles);
    }

    [Fact]
    public async Task CreateAsync_Should_Set_Correct_Expiration()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var result = await _provider.CreateAsync(
            Guid.NewGuid().ToString(),
            "john@test.com",
            ["Customer"]);

        var after = DateTime.UtcNow;

        // Assert
        result.ExpiresAt.Should().BeOnOrAfter(
            before.AddMinutes(_options.ExpirationInMinutes));

        result.ExpiresAt.Should().BeOnOrBefore(
            after.AddMinutes(_options.ExpirationInMinutes + 1));
    }

    [Fact]
    public async Task CreateAsync_Should_Generate_Different_Tokens_For_Same_User()
    {
        // Act
        var token1 = await _provider.CreateAsync(
            "1",
            "john@test.com",
            ["Customer"]);

        var token2 = await _provider.CreateAsync(
            "1",
            "john@test.com",
            ["Customer"]);

        // Assert
        token1.AccessToken.Should().NotBe(token2.AccessToken);
    }
}