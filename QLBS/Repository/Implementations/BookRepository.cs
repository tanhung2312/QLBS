using Microsoft.EntityFrameworkCore;
using QLBS.Constants;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly QLBSDbContext _context;

        public BookRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b => b.EntryDate)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .FirstOrDefaultAsync(b => b.BookId == id && b.IsDeleted == false);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            book.EntryDate = DateTime.Now;
            book.IsDeleted = false;
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;
            book.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BookExistsAsync(string title, int authorId)
        {
            return await _context.Books.AnyAsync(b =>
                b.BookTitle == title && b.AuthorId == authorId && !b.IsDeleted);
        }
        public async Task AddBookImageAsync(BookImage bookImage)
        {
            await _context.BookImages.AddAsync(bookImage);
            await _context.SaveChangesAsync();
        }

        public async Task<BookImage?> GetCoverImageAsync(int bookId)
        {
            return await _context.BookImages
                .FirstOrDefaultAsync(img => img.BookId == bookId && img.IsCover == true);
        }

        public async Task RemoveBookImageAsync(BookImage bookImage)
        {
            _context.BookImages.Remove(bookImage);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetTopSellingBooksAsync(int top = 8)
        {

            var topSellingData = await _context.OrderDetails
                .Where(od => od.Order.OrderStatus == OrderStatusConstants.Completed)
                .GroupBy(od => od.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(top)
                .ToListAsync();

            if (!topSellingData.Any()) return new List<Book>();

            var topBookIds = topSellingData.Select(x => x.BookId).ToList();


            var books = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.BookImages) 
                .Where(b => topBookIds.Contains(b.BookId) && b.IsDeleted == false)
                .ToListAsync();


            var result = topSellingData
                .Select(ts => books.FirstOrDefault(b => b.BookId == ts.BookId))
                .Where(b => b != null) 
                .ToList();

            return result!;
        }

        public async Task<IEnumerable<Book>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Books
                .AsNoTracking()
                .Where(b => b.CategoryId == categoryId && !b.IsDeleted)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.BookImages)
                .OrderByDescending(b => b.EntryDate)
                .ToListAsync();
        }

        public async Task<int> CountByCategoryIdAsync(int categoryId)
        {
            return await _context.Books
                .AsNoTracking()
                .CountAsync(b => b.CategoryId == categoryId && !b.IsDeleted);
        }

    }
}