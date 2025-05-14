using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Data.Configurations;

internal class LeagueConfiguration : IEntityTypeConfiguration<League> //cip...59. internal as only needed in this project. EntityTypeConfiguration is a class in the Microsoft.EntityFrameworkCore namespace that provides a way to configure an entity type in Entity Framework Core.
{
    public void Configure(EntityTypeBuilder<League> builder) 
    {
        builder.HasData(
          new League { Id = 1, Name = "Jamaica Premier League", CreatedDate = new DateTime(2025, 5, 14, 18, 0, 0), CreatedBy = "TestUser1" }, //hard-coding due to migration errors. DateTimeOffset.UtcNow.DateTime
          new League { Id = 2, Name = "English Premier League", CreatedDate = new DateTime(2025, 5, 14, 18, 0, 1), CreatedBy = "TestUser1" },
          new League { Id = 3, Name = "La Liga", CreatedDate = new DateTime(2025, 5, 14, 18, 0, 2), CreatedBy = "TestUser1" }
        );
    }
}