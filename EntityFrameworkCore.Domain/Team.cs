namespace EntityFrameworkCore.Domain;

public class Team : BaseDomainModel //cip...12
{
    public string? Name { get; set; }
    public int CoachId { get; set; }

    public League League { get; set; }
    public int LeagueId { get; set; }

    public List<Match> HomeMatches { get; set; } = new(); //cip...72
    public List<Match> AwayMatches { get; set; } = new(); //cip...72
}