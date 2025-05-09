using System;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Data;

public class FootballLeagueDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //cip...14
    {
        //base.OnConfiguring(optionsBuilder); //cip...16. default not needed.
        //using sqlserver
        //optionsBuilder.UseSqlServer("Data Source=localhost,1448;Initial Catalog=FootballLeague_EFCore;Encrypt=False;user id=sa;password=Str0ngPa$$w0rd;"); //to be tested
        optionsBuilder.UseSqlite($"Data Source=FootballLeague_EFCore.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) //cip...24
    {
        //base.OnModelCreating(modelBuilder); //cip...24. default not needed.
        modelBuilder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Tivoli Gardens FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 0) }, //hard-coding due to migration errors in .net 9 re changeable values eg DateTimeOffset.UtcNow.DateTime
            new Team { Id = 2, Name = "Waterhouse FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 0) },
            new Team { Id = 3, Name = "Humble Lions FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 0) }
        );
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Coach> Coaches { get; set; }
}