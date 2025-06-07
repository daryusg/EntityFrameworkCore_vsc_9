// Create a WebApplicationBuilder instance with default configuration
using EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register controller services to the container (enables [ApiController] support)
//builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Configure JSON serialization options to ignore cyclic references
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; //cip...104
}); //cip...104

var sqliteDatabaseName = builder.Configuration.GetConnectionString("SqliteDatabaseConnectionString"); //cip...99
//-----------------------------------------------------------
// copied from FootballLeagueDbContext.cs constructor
var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = Path.Combine(path, sqliteDatabaseName);
//-----------------------------------------------------------
var connectionString = $"Data Source={dbPath};"; //cip...99
//var connectionString = builder.Configuration.GetConnectionString("SqlServerDatabaseConnectionString"); //cip...99

builder.Services.AddDbContext<FootballLeagueDbContext>(options =>
{
    //options allows replacement of the FootballLeagueDbContext.OnConfiguring method //cip...99
    //options.UseSqlServer(sqlServerDatabaseConnectionString) //cip...99
    options.UseSqlite(connectionString, sqliteOptions =>
    {
        //sqliteOptions.MigrationsAssembly("EntityFrameworkCore.Data"); //auto-code
        //sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); //auto-code
        //sqliteOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo"); //tw mention
        sqliteOptions.CommandTimeout(30); //cip...115. Set command timeout to 30 seconds
        //NOTE: sqlite doesn't have enable retry on failure like SQL Server, so we don't need to configure that here.
    }) //cip...99
        //.UseLazyLoadingProxies()
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        .LogTo(Console.WriteLine, LogLevel.Information);

    if (!builder.Environment.IsProduction()) // Only enable sensitive data logging and detailed errors when NOT in Production
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});


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
