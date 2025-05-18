namespace EntityFrameworkCore.Domain;

public class Coach : BaseDomainModel //cip...12
{
    public string Name { get; set; }
    public virtual Team? Team { get; set; } //cip...74, 80 virtual
}