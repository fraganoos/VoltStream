using Scalar.AspNetCore;
using VoltStream.Application;
using VoltStream.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithTheme(ScalarTheme.BluePlanet);
        opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        opt.WithTitle("VoltStream API Documentation");
        opt.WithLayout(ScalarLayout.Modern);
    });
}

app.UseHttpsRedirection();

app.UseCors(s => s.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();
