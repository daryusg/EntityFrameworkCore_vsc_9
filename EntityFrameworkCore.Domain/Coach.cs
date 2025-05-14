namespace EntityFrameworkCore.Domain;

public class Coach : BaseDomainModel //cip...12
{
    public string? Name { get; set; }
    public int? TeamId { get; set; }
}