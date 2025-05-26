// Import the Swagger UI middleware from Swashbuckle
using Swashbuckle.AspNetCore.SwaggerUI;

// Create a WebApplicationBuilder instance with default configuration
var builder = WebApplication.CreateBuilder(args);

// Register controller services to the container (enables [ApiController] support)
builder.Services.AddControllers();

// Add support for discovering minimal API endpoints (used by Swagger)
builder.Services.AddEndpointsApiExplorer();

// Register the Swagger generator service (used to generate OpenAPI docs)
builder.Services.AddSwaggerGen();

// Build the WebApplication using the configured builder
var app = builder.Build();

// Configure middleware to run only in the Development environment
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger JSON
    app.UseSwagger();

    // Enable the Swagger UI middleware to serve the Swagger UI at /swagger
    app.UseSwaggerUI();
}

// Redirect all HTTP requests to HTTPS
app.UseHttpsRedirection();

// Enable authorization middleware (requires [Authorize] attribute or policies to take effect)
app.UseAuthorization();

// Map attribute-routed controllers (e.g., [Route("api/[controller]")])
app.MapControllers();

// Start the application and listen for HTTP requests
app.Run();
