# CONFIGURATION
$projectName = "EntityFrameworkCore.Api"
$framework = "net9.0"

# Find existing solution file in current directory
$solution = Get-ChildItem -Filter *.sln | Select-Object -First 1

if (-not $solution) {
    Write-Error "No solution file (*.sln) found in current directory."
    exit 1
}

Write-Host "Found solution: $($solution.Name)"

# Create webapi project
dotnet new webapi -n $projectName --framework $framework --no-https:$false

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create project."
    exit 1
}

# Add project to solution
dotnet sln $solution.FullName add "$projectName\$projectName.csproj"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to add project to solution."
    exit 1
}

# Overwrite Program.cs
$programCsPath = Join-Path $projectName "Program.cs"
@"
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
"@ | Set-Content -Path $programCsPath -Encoding UTF8

# Create Controllers directory
$controllersDir = Join-Path $projectName "Controllers"
if (-not (Test-Path $controllersDir)) {
    New-Item -Path $controllersDir -ItemType Directory | Out-Null
}

# Create WeatherForecastController.cs
$controllerPath = Join-Path $controllersDir "WeatherForecastController.cs"
@"
using Microsoft.AspNetCore.Mvc;

namespace $projectName.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();
    }

    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
"@ | Set-Content -Path $controllerPath -Encoding UTF8

Write-Host "✅ Project '$projectName' created and added to solution '$($solution.Name)'."
Write-Host "▶ To run it:"
Write-Host "   cd $projectName"
Write-Host "   dotnet run"
