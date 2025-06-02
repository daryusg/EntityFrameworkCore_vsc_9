using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Domain;

public class Coach : BaseDomainModel //cip...12
{
    [MaxLength(50)] //cip...110
    [Required] //cip...110
    public string Name { get; set; }
    public virtual Team? Team { get; set; } //cip...74, 80 virtual
}