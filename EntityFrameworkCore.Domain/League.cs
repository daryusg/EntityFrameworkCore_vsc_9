using System;

namespace EntityFrameworkCore.Domain;

public class League : BaseDomainModel //cip...57
{
    public string Name { get; set; } = string.Empty;
    public int LeagueId { get; set; }
    public int CoachId { get; set; }
}
