using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShopSphere.Api.Authentication;
using ShopSphere.Infrastructure.Authentication;
using System.Text;

namespace ShopSphere.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<JwtBearerEventsConfiguration>();

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                        context.HttpContext.RequestServices
                            .GetRequiredService<JwtBearerEventsConfiguration>()
                            .HandleChallenge(context),

                    OnForbidden = context =>
                        context.HttpContext.RequestServices
                            .GetRequiredService<JwtBearerEventsConfiguration>()
                            .HandleForbidden(context)
                };
            });

        services.AddAuthorization();

        return services;
    }
}