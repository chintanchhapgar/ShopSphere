using Microsoft.AspNetCore.Identity;
using ShopSphere.Domain.Constants;
using ShopSphere.Infrastructure.Identity;

namespace ShopSphere.Infrastructure.Persistence.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        RoleManager<ApplicationRole> roleManager)
    {
        string[] roles =
        {
            Roles.Admin,
            Roles.Vendor,
            Roles.Customer
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new ApplicationRole
                    {
                        Name = role
                    });
            }
        }
    }
}