using System;

namespace EntityFrameworkCore.Domain;

public class LeaguesAndTeamsView //cip...88
{
    public int LeagueId { get; set; }
    public string LeagueName { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int CoachId { get; set; }
    public string CoachName { get; set; } = string.Empty;

}
