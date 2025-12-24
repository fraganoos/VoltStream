using VoltStream.Infrastructure;
using VoltStream.WebApi;
using VoltStream.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Service registrations
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlerMiddleware>(); // Global exception handling
app.UseVoltStreamPipeline(); // HTTPS, CORS, Auth
app.UseOpenApiDocumentation(); // Scalar UI

await app.UseInfrastructureDatabase();

app.MapControllers();

app.Run();
