using System;

namespace EntityFrameworkCore.Domain;

public abstract class BaseDomainModel
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } //cip...57
    public DateTime? ModifiedDate { get; set; } //cip...57. null at creation
    public string? ModifiedBy { get; set; } //cip...57. null at creation
}