using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Data;

public class FootballLeagueDbContext : DbContext
{
    public FootballLeagueDbContext() //NOTE: ctor<tab> created "Parameters" parameter(??)
    {
        //cip...29. set the path to the database file in the local application data folder.
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder); //12/05/25. from chatgpt: C:\Users\<YourUsername>\AppData\Local\
        DbPath = Path.Combine(path, "FootballLeague_EFCore.db"); //<ctrl>. -> "Generate property DbPath"
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //base.OnConfiguring(optionsBuilder); //cip...16. default not needed.
        //using sqlserver
        //optionsBuilder.UseSqlServer("Data Source=localhost,1448;Initial Catalog=FootballLeague_EFCore;Encrypt=False;user id=sa;password=Str0ngPa$$w0rd;"); //to be tested
        //optionsBuilder.UseSqlite($"Data Source=FootballLeague_EFCore.db");
        optionsBuilder.UseSqlite($"Data Source={DbPath}")
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) //cip...42. set the default tracking behavior to NoTracking.
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors(); //run dotnet ef database update --startup-project ./EntityFrameworkCore.Console --project ./EntityFrameworkCore.Data (vs code terminal) to create the database file (in the common location).
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Team>().HasData(
          new Team { Id = 1, Name = "Tivoli Gardens FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 0) }, //hard-coding due to migration errors. DateTimeOffset.UtcNow.DateTime
          new Team { Id = 2, Name = "Waterhouse FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 1) },
          new Team { Id = 3, Name = "Humble Lions FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 2) }
        );
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public string DbPath { get; private set; }
}