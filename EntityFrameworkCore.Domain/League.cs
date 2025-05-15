using System;

namespace EntityFrameworkCore.Domain;

public class League : BaseDomainModel //cip...57
{
    public string Name { get; set; } = string.Empty;
    public List<Team> Teams { get; set; }
}
