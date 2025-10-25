using VoltStream.WebApi;
using VoltStream.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Service registrations
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlerMiddleware>(); // Global exception handling
app.UseInfrastructure(); // HTTPS, CORS, Auth
app.UseOpenApiDocumentation(); // Scalar UI

app.MapControllers();

app.Run();
