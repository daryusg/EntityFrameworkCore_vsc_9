using System;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Data.Configurations;

internal class TeamConfiguration : IEntityTypeConfiguration<Team> //cip...59. internal as only needed in this project. EntityTypeConfiguration is a class in the Microsoft.EntityFrameworkCore namespace that provides a way to configure an entity type in Entity Framework Core.
{
    public void Configure(EntityTypeBuilder<Team> builder) 
    {
        builder.HasData(
          new Team { Id = 1, Name = "Tivoli Gardens FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 0), CreatedBy = "TestUser1" }, //hard-coding due to migration errors. DateTimeOffset.UtcNow.DateTime
          new Team { Id = 2, Name = "Waterhouse FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 1), CreatedBy = "TestUser1" },
          new Team { Id = 3, Name = "Humble Lions FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 2), CreatedBy = "TestUser1" }
        ); //cip...24. cip...58 added for new col , CreatedBy = "TestUser1"
    }
}
