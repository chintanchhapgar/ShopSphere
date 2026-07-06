using ShopSphere.Application;
using ShopSphere.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Register application services
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();