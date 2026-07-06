using Microsoft.AspNetCore.Identity;
using ShopSphere.Api.Endpoints.Authentication;
using ShopSphere.Application;
using ShopSphere.Application.Features.Authentication.Register;
using ShopSphere.Infrastructure;
using ShopSphere.Infrastructure.Identity;
using ShopSphere.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Register application services
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapAuthenticationEndpoints();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<ApplicationRole>>();

    await RoleSeeder.SeedAsync(roleManager);
}

app.Run();