using System;

namespace EntityFrameworkCore.Domain;

public abstract class BaseDomainModel
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
}