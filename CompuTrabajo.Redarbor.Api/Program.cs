using CompuTrabajo.Redarbor.Application.Command;
using CompuTrabajo.Redarbor.Application.Command.CommandHandlers;
using CompuTrabajo.Redarbor.Application.Common;
using CompuTrabajo.Redarbor.Application.Common.Dto;
using CompuTrabajo.Redarbor.Application.Common.Interfaces;
using CompuTrabajo.Redarbor.Application.Common.Interfaces.Persistance;
using CompuTrabajo.Redarbor.Application.Query;
using CompuTrabajo.Redarbor.Application.Query.QueryHandlers;
using CompuTrabajo.Redarbor.Domain.Employees;
using CompuTrabajo.Redarbor.Infrastruture.Persistance;
using CompuTrabajo.Redarbor.Infrastruture.Persistance.Extensions;
using CompuTrabajo.Redarbor.Infrastruture.Persistance.Extensions.Configuration;
using CompuTrabajo.Redarbor.Infrastruture.Persistance.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
// using Microsoft.OpenApi.Models; // removed: not available in this project reference

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Repositories
builder.Services.AddScoped<IRepository<Employee>, EmployeesRepository>();
builder.Services.AddScoped<IReadRepository<EmployeeReadDto>, EmployeeReadRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

//Commands
builder.Services.AddScoped<ICommandHandler<CreateEmployeeCommand>, CreateEmployeeCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateEmployeeCommand>, UpdateEmployeeCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteEmployeeCommand>, DeleteEmployeeCommandHandler>();

//Query
builder.Services.AddScoped<IQueryHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeReadDto>>, GetAllEmployeesQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetEmployeeQuery,EmployeeReadDto>, GetEmployeeQueryHandler>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    // Security requirement not added here because the project's OpenAPI types differ by package version.
    // Keep the security definition so Swagger UI shows the Bearer input.
});


builder.Services.AddSqlServerPersistence<RedarborDbContext>(
    builder.Configuration,
    new SqlServerPersistenceOptions
    {
        // Match the connection string key from appsettings.json
        ConnectionStringName = "Default",
        CommandTimeoutSeconds = 30,
        EnableDetailedErrors = true,
        EnableSensitiveDataLogging = false
    });

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database");

var jwtSettings = builder.Configuration.GetSection("Jwt");

var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Apply pending EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<RedarborDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoints
app.MapHealthChecks("/health/live"); // Liveness probe

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }
});

app.MapControllers();

app.Run();

// Health check implementation
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly RedarborDbContext _db;

    public DatabaseHealthCheck(RedarborDbContext db)
    {
        _db = db;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(cancellationToken);
            return canConnect ? HealthCheckResult.Healthy("Database reachable") : HealthCheckResult.Unhealthy("Cannot connect to database");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Exception checking database", ex);
        }
    }
}

