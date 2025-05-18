namespace EntityFrameworkCore.Domain;

public class League : BaseDomainModel //cip...57
{
    public string Name { get; set; } = string.Empty;
    public virtual List<Team> Teams { get; set; } = []; //cip...78, 80 virtual
}
