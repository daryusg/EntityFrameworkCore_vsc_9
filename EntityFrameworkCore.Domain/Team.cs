using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Domain;

public class Team : BaseDomainModel //cip...12
{
    public string? Name { get; set; }
    
    public virtual Coach Coach { get; set; } //cip...74, 80 virtual
    public int CoachId { get; set; }

    public virtual League? League { get; set; } //cip...80 virtual
    public int? LeagueId { get; set; }

    //for sql server only. see BaseDomainModel
    // [Timestamp] //cip...113.
    // public byte[] RowVersion { get; set; } = Array.Empty<byte>(); //cip...113. used for concurrency control.

    //for sqlite and sqlserver. see BaseDomainModel
    // [ConcurrencyCheck]
    // public Guid RowGuid { get; set; } //cip...113. used for concurrency control. This is a unique identifier for the row that is used to detect concurrency conflicts.

    public virtual List<Match> HomeMatches { get; set; } = new(); //cip...72, 80 virtual
    public virtual List<Match> AwayMatches { get; set; } = new(); //cip...72, 80 virtual
}