using System;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Data.Configurations;

internal class TeamConfiguration : IEntityTypeConfiguration<Team> //cip...59. internal as only needed in this project. EntityTypeConfiguration is a class in the Microsoft.EntityFrameworkCore namespace that provides a way to configure an entity type in Entity Framework Core.
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasIndex(q => q.Name).IsUnique(); //cip...72. Added to ensure that team names are unique in the database.

        //---------------------------------------------------------------------------
        //cip...72. set up the many-to-many relationship
        //---------------------------------------------------------------------------
        builder.HasMany(m => m.HomeMatches)
            .WithOne(q => q.HomeTeam)
            .HasForeignKey(q => q.HomeTeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); //cip...72. Added to ensure that if a team is deleted, the home matches are not deleted.

        builder.HasMany(m => m.AwayMatches)
            .WithOne(q => q.AwayTeam)
            .HasForeignKey(q => q.AwayTeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); //cip...72. Added to ensure that if a team is deleted, the home matches are not deleted.
        //---------------------------------------------------------------------------

        builder.HasData(
          new Team { Id = 1, Name = "Tivoli Gardens FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 0), CreatedBy = "TestUser1" }, //hard-coding due to migration errors. DateTimeOffset.UtcNow.DateTime
          new Team { Id = 2, Name = "Waterhouse FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 1), CreatedBy = "TestUser1" },
          new Team { Id = 3, Name = "Humble Lions FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 2), CreatedBy = "TestUser1" }
        ); //cip...24. cip...58 added for new col , CreatedBy = "TestUser1"
    }
}
