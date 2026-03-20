using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class LibraryBook : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public string? Publisher { get; set; }
    public int? PublicationYear { get; set; }
    public string? Category { get; set; }
    public string? CoverImage { get; set; }
    public string? Description { get; set; }
    public string? ShelfLocation { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int BranchId { get; set; }
    public string? Barcode { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<BookBorrow> Borrows { get; set; } = new List<BookBorrow>();
}

public class BookBorrow : BaseEntity
{
    public int LibraryBookId { get; set; }
    public int PersonId { get; set; }
    public PersonType PersonType { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowStatus Status { get; set; }
    public decimal? FineAmount { get; set; }
    public bool FinePaid { get; set; }
    public string? Notes { get; set; }

    public virtual LibraryBook LibraryBook { get; set; } = null!;
}
