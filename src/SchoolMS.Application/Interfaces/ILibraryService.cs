using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ILibraryService
{
    Task<List<LibraryBookDto>> GetAllBooksAsync();
    Task<List<LibraryBookDto>> GetBooksBySchoolIdAsync(int schoolId, int? branchId = null);
    Task<LibraryBookDto?> GetBookByIdAsync(int id);
    Task<LibraryBookDto> CreateBookAsync(LibraryBookDto dto);
    Task<LibraryBookDto> UpdateBookAsync(LibraryBookDto dto);
    Task DeleteBookAsync(int id);
    Task<List<BookBorrowDto>> GetAllBorrowsAsync();
    Task<BookBorrowDto> BorrowBookAsync(BookBorrowDto dto);
    Task ReturnBookAsync(int borrowId);
}

