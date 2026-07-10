using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using ShopSphere.Api.Endpoints;
using ShopSphere.Api.Extensions;
using ShopSphere.Api.Middlewares;
using ShopSphere.Application;
using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure;
using ShopSphere.Infrastructure.Email.Models;
using ShopSphere.Infrastructure.Email.Settings;
using ShopSphere.Infrastructure.Identity;
using ShopSphere.Infrastructure.Persistence;
using ShopSphere.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));

builder.Services.Configure<EmailTemplateOptions>(
    builder.Configuration.GetSection("EmailTemplates"));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShopSphere API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<ApplicationRole>>();

    await RoleSeeder.SeedAsync(roleManager);
}

app.Run();