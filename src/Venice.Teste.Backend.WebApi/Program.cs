using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Venice.Teste.Backend.Application.DependencyInjection;
using Venice.Teste.Backend.WebApi.Extensions;
using Venice.Teste.Backend.WebApi.Middleware;
using Venice.Teste.Backend.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// App layers and Infra
builder.Services.AddDependencyApplicationSetup(builder.Configuration);

// Swagger + API explorer + versioning
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEssentials();
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Authentication & Authorization (JWT)
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? "venice.local";
var audience = jwtSection["Audience"] ?? "venice.api";
var secret = jwtSection["Secret"] ?? "super_secret_dev_key_please_change";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = key
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure SQL database is created when not using InMemory
if (!app.Configuration.GetValue<bool>("UseInMemoryDatabase"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("PermitirTudo");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Global error handling
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

// Swagger
app.ConfigureSwagger();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
    endpoints.MapHealthChecks("/ready");
});

app.Run();