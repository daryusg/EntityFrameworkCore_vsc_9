using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Data.Configurations;

internal class CoachConfiguration : IEntityTypeConfiguration<Coach> //cip...59. internal as only needed in this project. EntityTypeConfiguration is a class in the Microsoft.EntityFrameworkCore namespace that provides a way to configure an entity type in Entity Framework Core.
{
    public void Configure(EntityTypeBuilder<Coach> builder) 
    {
        builder.HasData(
          new Coach { Id = 1, Name = "Jose Mourinho", CreatedDate = new DateTime(2025, 5, 17, 11, 0, 0), CreatedBy = "TestUser1" }, //hard-coding due to migration errors. DateTimeOffset.UtcNow.DateTime
          new Coach { Id = 2, Name = "Josep \"Pep\" Guardiola Sala", CreatedDate = new DateTime(2025, 5, 17, 11, 0, 1), CreatedBy = "TestUser1" },
          new Coach { Id = 3, Name = "Trevior Williams", CreatedDate = new DateTime(2025, 5, 17, 11, 0, 2), CreatedBy = "TestUser1" }
        );
    }
}