using System.Reflection;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Data;

public class FootballLeagueDbContext : DbContext
{
    //cip...100  public FootballLeagueDbContext() //NOTE: ctor<tab> created "Parameters" parameter(??)
    // {
    //     //cip...29. set the path to the database file in the local application data folder.
    //     var folder = Environment.SpecialFolder.LocalApplicationData;
    //     var path = Environment.GetFolderPath(folder); //12/05/25. from chatgpt: C:\Users\<YourUsername>\AppData\Local\
    //     //DbPath = Path.Combine(path, "FootballLeague_EFCore.db"); //"DBPath" -> <ctrl .> -> "Generate property DbPath"
    //     DbPath = Path.Combine(path, "FootballLeague_EFCoreFor61.db"); //"DBPath" -> <ctrl .> -> "Generate property DbPath" //cip...61
    // }

    public FootballLeagueDbContext(DbContextOptions<FootballLeagueDbContext> options) : base(options) //cip...99. constructor with options parameter for dependency injection.
    {
    }

    //cip...100  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     //base.OnConfiguring(optionsBuilder); //cip...16. default not needed.
    //     //using sqlserver
    //     //optionsBuilder.UseSqlServer("Data Source=localhost,1448;Initial Catalog=FootballLeague_EFCore;Encrypt=False;user id=sa;password=Str0ngPa$$w0rd;"); //to be tested
    //     //optionsBuilder.UseSqlite($"Data Source=FootballLeague_EFCore.db");
    //     optionsBuilder.UseSqlite($"Data Source={DbPath}")
    //         .UseLazyLoadingProxies() //cip...80
    //                                  //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) //cip...42. set the default tracking behavior to NoTracking.
    //         .LogTo(Console.WriteLine, LogLevel.Information)
    //         .EnableSensitiveDataLogging()
    //         .EnableDetailedErrors(); //run dotnet ef database update --startup-project ./EntityFrameworkCore.Console --project ./EntityFrameworkCore.Data (vs code terminal) to create the database file (in the common location).
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfiguration(new TeamConfiguration()); //cip...59
        //modelBuilder.ApplyConfiguration(new LeagueConfiguration()); //cip...59
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); //cip...59. apply all configurations in the assembly.
        modelBuilder.Entity<LeaguesAndTeamsView>().HasNoKey().ToView("vw_LeaguesAndTeams"); //cip...88. view for leagues and teams.
        modelBuilder.HasDbFunction(typeof(FootballLeagueDbContext).GetMethod(nameof(GetEarliestTeamMatch), new[] { typeof(int) })).HasName("fn_GetEarliestMatch"); //cip...92.
    }

    public DateTime GetEarliestTeamMatch(int teamId) => throw new NotImplementedException(); //cip...92

    public DbSet<Team> Teams { get; set; } //cip...12
    public DbSet<Coach> Coaches { get; set; } //cip...12
    public DbSet<League> Leagues { get; set; } //cip...57
    public DbSet<Match> Matches { get; set; } //cip...57
    public DbSet<LeaguesAndTeamsView> LeaguesAndTeams_View { get; set; } //cip...88. view for leagues and teams.
    public string DbPath { get; private set; }
}

public class FootbalLeagueDbContextFactory : IDesignTimeDbContextFactory<FootballLeagueDbContext> //cip...100. This class is used to create a DbContext instance at design time, such as during migrations or scaffolding.
{
    public FootballLeagueDbContext CreateDbContext(string[] args)
    {
        //-----------------------------------------------------------
        // copied from FootballLeagueDbContext.cs constructor
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        //-----------------------------------------------------------
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        var dbPath = Path.Combine(path, configuration.GetConnectionString("SqliteDatabaseConnectionString"));
        var sqlServerDatabaseConnectionString = configuration.GetConnectionString("SqliteDatabaseConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<FootballLeagueDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        //optionsBuilder.UseSqlServer(sqlServerDatabaseConnectionString);

        return new FootballLeagueDbContext(optionsBuilder.Options);
    }
}