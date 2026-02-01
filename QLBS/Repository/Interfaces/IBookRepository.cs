using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(string title, int authorId);
        Task AddBookImageAsync(BookImage bookImage);
        Task<BookImage?> GetCoverImageAsync(int bookId);
        Task RemoveBookImageAsync(BookImage bookImage);
    }
}