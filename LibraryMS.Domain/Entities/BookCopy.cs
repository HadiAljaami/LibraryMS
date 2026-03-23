using LibraryMS.Domain.Common;
using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Entities;

public class BookCopy : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public string CopyNumber { get; set; } = string.Empty;
    public CopyStatus Status { get; set; } = CopyStatus.Available;
    public DateTime AcquiredDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}