using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure.Authentication;
using ShopSphere.Infrastructure.Identity;
using ShopSphere.Infrastructure.Persistence;
using System.Text;

namespace ShopSphere.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddHttpContextAccessor();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

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
        });

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddAuthorization();

        return services;
    }
}