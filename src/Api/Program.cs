using Api.Endpoints;
using Api.Infrastructure;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Migrations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost\\SQLEXPRESS;Database=LEMinimalAPIDB;User Id=sqluser;Password=password;TrustServerCertificate=True;";

// Composition root: wire up each Clean Architecture layer.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

// Cross-cutting API concerns.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// OpenAPI document + Scalar interactive docs.
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

// Apply migrations and seed default data on startup (handled by the Migrations project).
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
        options.WithTitle("Clean Architecture API").WithTheme(ScalarTheme.Purple));
}

app.MapOrganizationEndpoints();

app.Run();

// Exposed so the WebApplicationFactory-based integration/functional tests can reference the entry point.
public partial class Program;
