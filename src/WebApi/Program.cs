using Application.Extensions;
using Infrastructure.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// ✅ Load environment variables from .env
// --------------------------------------------------
// Env.Load();
Env.Load("../../.env");

Console.WriteLine($"[DEBUG] DB_HOST: {Environment.GetEnvironmentVariable("DB_HOST")}");
Console.WriteLine($"[DEBUG] DB_NAME: {Environment.GetEnvironmentVariable("DB_NAME")}");
Console.WriteLine($"[DEBUG] DB_USER: {Environment.GetEnvironmentVariable("DB_USER")}");
Console.WriteLine($"[DEBUG] DB_PASSWORD: {Environment.GetEnvironmentVariable("DB_PASSWORD")}");


// Read from environment
var host = Environment.GetEnvironmentVariable("DB_HOST");
var db = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Build connection string
var connectionString = $"Host={host};Database={db};Username={user};Password={pass}";

// Inject into configuration before Infrastructure uses it
builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// --------------------------------------------------
// Add services to the container
// --------------------------------------------------
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<Application.Contracts.DTOs.Search.SearchBusInputDtoValidator>();
        fv.DisableDataAnnotationsValidation = true;
    });

// Add Infrastructure layer (DbContext, Repositories, UnitOfWork)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application layer (Services, AutoMapper, Domain Services)
builder.Services.AddApplication();

// --------------------------------------------------
// Configure CORS
// --------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular default port
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// --------------------------------------------------
// Swagger / OpenAPI setup
// --------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bus Ticket Reservation API",
        Version = "v1",
        Description = "API for Bus Ticket Reservation System with DDD and Clean Architecture"
    });

    // Enable XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// --------------------------------------------------
// Configure the HTTP request pipeline
// --------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bus Ticket Reservation API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();
app.Run();
