using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class LibraryService : ILibraryService
{
    private readonly IRepository<LibraryBook> _bookRepo;
    private readonly IRepository<BookBorrow> _borrowRepo;
    private readonly IUnitOfWork _unitOfWork;

    public LibraryService(IRepository<LibraryBook> bookRepo, IRepository<BookBorrow> borrowRepo, IUnitOfWork unitOfWork)
    { _bookRepo = bookRepo; _borrowRepo = borrowRepo; _unitOfWork = unitOfWork; }

    public async Task<List<LibraryBookDto>> GetAllBooksAsync()
    {
        var items = await _bookRepo.Query().Include(b => b.Branch).ToListAsync();
        return items.Select(b => new LibraryBookDto
        {
            Id = b.Id, Title = b.Title, Author = b.Author, ISBN = b.ISBN, Publisher = b.Publisher,
            Category = b.Category, ShelfLocation = b.ShelfLocation, TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies, BranchId = b.BranchId, BranchName = b.Branch?.Name, Barcode = b.Barcode,
            SchoolId = b.SchoolId
        }).ToList();
    }

    public async Task<List<LibraryBookDto>> GetBooksBySchoolIdAsync(int schoolId, int? branchId = null)
    {
        var query = _bookRepo.Query().Where(b => b.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(b => b.BranchId == branchId.Value);
        var items = await query.Include(b => b.Branch).ToListAsync();
        return items.Select(b => new LibraryBookDto
        {
            Id = b.Id, Title = b.Title, Author = b.Author, ISBN = b.ISBN, Publisher = b.Publisher,
            Category = b.Category, ShelfLocation = b.ShelfLocation, TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies, BranchId = b.BranchId, BranchName = b.Branch?.Name, Barcode = b.Barcode,
            SchoolId = b.SchoolId
        }).ToList();
    }

    public async Task<LibraryBookDto?> GetBookByIdAsync(int id)
    {
        var b = await _bookRepo.Query().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return null;
        return new LibraryBookDto
        {
            Id = b.Id, Title = b.Title, Author = b.Author, ISBN = b.ISBN, Publisher = b.Publisher,
            Category = b.Category, ShelfLocation = b.ShelfLocation, TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies, BranchId = b.BranchId, BranchName = b.Branch?.Name, Barcode = b.Barcode,
            SchoolId = b.SchoolId
        };
    }

    public async Task<LibraryBookDto> CreateBookAsync(LibraryBookDto dto)
    {
        var entity = new LibraryBook
        {
            Title = dto.Title, Author = dto.Author, ISBN = dto.ISBN, Publisher = dto.Publisher,
            Category = dto.Category, ShelfLocation = dto.ShelfLocation, TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.TotalCopies, BranchId = dto.BranchId, Barcode = dto.Barcode,
            SchoolId = dto.SchoolId
        };
        await _bookRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; dto.AvailableCopies = entity.AvailableCopies; return dto;
    }

    public async Task<LibraryBookDto> UpdateBookAsync(LibraryBookDto dto)
    {
        var entity = await _bookRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.Title = dto.Title; entity.Author = dto.Author; entity.ISBN = dto.ISBN; entity.Publisher = dto.Publisher;
        entity.Category = dto.Category; entity.ShelfLocation = dto.ShelfLocation;
        entity.TotalCopies = dto.TotalCopies; entity.Barcode = dto.Barcode;
        entity.SchoolId = dto.SchoolId;
        _bookRepo.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteBookAsync(int id)
    {
        var e = await _bookRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _bookRepo.Update(e); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<BookBorrowDto>> GetAllBorrowsAsync()
    {
        var items = await _borrowRepo.Query().Include(b => b.LibraryBook).OrderByDescending(b => b.BorrowDate).ToListAsync();
        return items.Select(b => new BookBorrowDto
        {
            Id = b.Id, LibraryBookId = b.LibraryBookId, BookTitle = b.LibraryBook?.Title,
            PersonId = b.PersonId, PersonType = b.PersonType, BorrowDate = b.BorrowDate,
            DueDate = b.DueDate, ReturnDate = b.ReturnDate, Status = b.Status, FineAmount = b.FineAmount
        }).ToList();
    }

    public async Task<BookBorrowDto> BorrowBookAsync(BookBorrowDto dto)
    {
        var book = await _bookRepo.GetByIdAsync(dto.LibraryBookId) ?? throw new KeyNotFoundException("Book not found");
        if (book.AvailableCopies <= 0) throw new InvalidOperationException("No copies available");

        book.AvailableCopies--;
        _bookRepo.Update(book);

        var entity = new BookBorrow
        {
            LibraryBookId = dto.LibraryBookId, PersonId = dto.PersonId, PersonType = dto.PersonType,
            BorrowDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14), Status = BorrowStatus.Borrowed
        };
        await _borrowRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; dto.BorrowDate = entity.BorrowDate; dto.DueDate = entity.DueDate; return dto;
    }

    public async Task ReturnBookAsync(int borrowId)
    {
        var borrow = await _borrowRepo.GetByIdAsync(borrowId) ?? throw new KeyNotFoundException();
        borrow.ReturnDate = DateTime.UtcNow; borrow.Status = BorrowStatus.Returned;
        if (borrow.ReturnDate > borrow.DueDate)
            borrow.FineAmount = (decimal)(borrow.ReturnDate.Value - borrow.DueDate).TotalDays * 1;
        _borrowRepo.Update(borrow);

        var book = await _bookRepo.GetByIdAsync(borrow.LibraryBookId);
        if (book != null) { book.AvailableCopies++; _bookRepo.Update(book); }
        await _unitOfWork.SaveChangesAsync();
    }
}

