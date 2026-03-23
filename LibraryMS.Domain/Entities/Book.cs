using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public string Language { get; set; } = "العربية";
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}