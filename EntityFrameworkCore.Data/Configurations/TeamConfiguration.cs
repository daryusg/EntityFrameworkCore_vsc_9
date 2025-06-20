using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team> //cip...59. internal as only needed in this project. EntityTypeConfiguration is a class in the Microsoft.EntityFrameworkCore namespace that provides a way to configure an entity type in Entity Framework Core.
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        //NOTE: all constraints that can be added at the database level can be added here (in the configuration). cip...110
        builder.HasIndex(q => q.Name).IsUnique(); //cip...72. Added to ensure that team names are unique in the database.
        //composite key configuration.
        //builder.HasIndex(q => new { q.CoachId, q.LeagueId }).IsUnique(); //cip...110

        builder.Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(100); //cip...110. this overides eg the name's nullability and the default max length of 450 characters for string properties in EF Core.

        //for sql server only
        // builder.Property(q => q.RowVersion)
        //     .IsRowVersion(); //cip...113. this is used for concurrency control. It will be automatically updated by the database when the row is updated.

        //for sqlite and sqlserver. equivalent to [ConcurrencyCheck] attribute.
        builder.Property(q => q.RowGuid)
            .IsConcurrencyToken(); //cip...113. this is used for concurrency control. It will be automatically updated by the database when the row is updated.

        builder.ToTable("Teams", t => t.IsTemporal()); //cip...109. "Teams" table is temporal, meaning it will keep track of historical data changes. This is useful for auditing and tracking changes over time.

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
          new Team { Id = 1, Name = "Tivoli Gardens FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 0), CreatedBy = "TestUser1", LeagueId = 1, CoachId = 1 }, //hard-coding due to migration errors. DateTimeOffset.UtcNow.DateTime
          new Team { Id = 2, Name = "Waterhouse FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 1), CreatedBy = "TestUser1", LeagueId = 1, CoachId = 2 },
          new Team { Id = 3, Name = "Humble Lions FC", CreatedDate = new DateTime(2025, 5, 9, 18, 0, 2), CreatedBy = "TestUser1", LeagueId = 1, CoachId = 3 }
        ); //cip...24. cip...58 added for new col , CreatedBy = "TestUser1"
    }
}
