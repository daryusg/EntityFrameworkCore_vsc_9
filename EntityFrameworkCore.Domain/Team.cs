namespace EntityFrameworkCore.Domain;

public class Team : BaseDomainModel //cip...12
{
    public string? Name { get; set; }
    
    public virtual Coach Coach { get; set; } //cip...74, 80 virtual
    public int CoachId { get; set; }

    public virtual League? League { get; set; } //cip...80 virtual
    public int? LeagueId { get; set; }

    public virtual List<Match> HomeMatches { get; set; } = new(); //cip...72, 80 virtual
    public virtual List<Match> AwayMatches { get; set; } = new(); //cip...72, 80 virtual
}