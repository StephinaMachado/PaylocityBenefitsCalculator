using Microsoft.OpenApi.Models;
using Paylocity.BenefitsCalculator.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Logging.AddConsole();
builder.Logging.ClearProviders();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureServices();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee benefit cost calculator",
        Description = "Api for benefit calculation"
    });
});

var allowLocalHost = "allow localhost";

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowLocalHost,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost");
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowLocalHost);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
