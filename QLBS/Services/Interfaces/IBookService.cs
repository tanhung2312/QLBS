using QLBS.Dtos.Book;

namespace QLBS.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponseDto>> GetAllBooksAsync();
        Task<BookResponseDto?> GetBookByIdAsync(int id);
        Task<BookResponseDto?> CreateBookAsync(CreateBookDto createDto);
        Task<bool> UpdateBookAsync(int id, UpdateBookDto updateDto);
        Task<bool> DeleteBookAsync(int id);
    }
}