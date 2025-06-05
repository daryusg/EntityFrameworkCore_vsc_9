using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Domain;

public abstract class BaseDomainModel
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } //cip...57
    public DateTime? ModifiedDate { get; set; } //cip...57. null at creation
    public string? ModifiedBy { get; set; } //cip...57. null at creation

    //for sql server only.
    // [Timestamp] //cip...113.
    // public byte[] RowVersion { get; set; } = Array.Empty<byte>(); //cip...113. used for concurrency control.

    //for sqlite and sqlserver
    [ConcurrencyCheck] //equivalent to builder.Property(q => q.RowGuid).IsConcurrencyToken();
    public Guid RowGuid { get; set; } //cip...113. used for concurrency control. This is a unique identifier for the row that is used to detect concurrency conflicts.
}