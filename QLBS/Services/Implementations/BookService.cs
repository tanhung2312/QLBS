using QLBS.Dtos.Book;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IPhotoService _photoService;

        public BookService(IBookRepository bookRepository, IPhotoService photoService)
        {
            _bookRepository = bookRepository;
            _photoService = photoService;
        }

        public async Task<IEnumerable<BookResponseDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();

            return books.Select(b => new BookResponseDto
            {
                BookId = b.BookId,
                BookTitle = b.BookTitle,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.CategoryName ?? "N/A",
                AuthorId = b.AuthorId,
                AuthorName = b.Author?.AuthorName ?? "N/A",
                PublishYear = b.PublishYear,
                Publisher = b.Publisher,
                Price = b.Price,
                Quantity = b.Quantity,
                Description = b.Description,
                ImageUrl = b.BookImages?.FirstOrDefault(img => img.IsCover == true)?.URL
                           ?? b.BookImages?.FirstOrDefault()?.URL
            });
        }

        public async Task<BookResponseDto?> GetBookByIdAsync(int id)
        {
            var b = await _bookRepository.GetBookByIdAsync(id);
            if (b == null) return null;

            return new BookResponseDto
            {
                BookId = b.BookId,
                BookTitle = b.BookTitle,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.CategoryName ?? "N/A",
                AuthorId = b.AuthorId,
                AuthorName = b.Author?.AuthorName ?? "N/A",
                PublishYear = b.PublishYear,
                Publisher = b.Publisher,
                Price = b.Price,
                Quantity = b.Quantity,
                Description = b.Description,
                ImageUrl = b.BookImages?.FirstOrDefault(img => img.IsCover == true)?.URL
            };
        }

        public async Task<BookResponseDto?> CreateBookAsync(CreateBookDto createDto)
        {
            if (await _bookRepository.BookExistsAsync(createDto.BookTitle, createDto.AuthorId))
                return null;

            var book = new Book
            {
                BookTitle = createDto.BookTitle,
                CategoryId = createDto.CategoryId,
                AuthorId = createDto.AuthorId,
                PublishYear = createDto.PublishYear,
                Publisher = createDto.Publisher,
                Price = createDto.Price,
                Quantity = createDto.Quantity,
                Description = createDto.Description
            };

            try
            {
                var newBook = await _bookRepository.AddBookAsync(book);
                string? imageUrl = null;

                if (createDto.ImageFile != null)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(createDto.ImageFile);
                    if (uploadResult.Error == null)
                    {
                        imageUrl = uploadResult.SecureUrl.ToString();

                        var bookImage = new BookImage
                        {
                            BookId = newBook.BookId,
                            URL = imageUrl,
                            IsCover = true,
                            OrderIndex = 1,
                            Description = "Cover Image"
                        };
                        await _bookRepository.AddBookImageAsync(bookImage);
                    }
                }

                return new BookResponseDto
                {
                    BookId = newBook.BookId,
                    BookTitle = newBook.BookTitle,
                    CategoryId = newBook.CategoryId,
                    AuthorId = newBook.AuthorId,
                    PublishYear = newBook.PublishYear,
                    Publisher = newBook.Publisher,
                    Price = newBook.Price,
                    Quantity = newBook.Quantity,
                    Description = newBook.Description,
                    ImageUrl = imageUrl
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto updateDto)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null) return false;
            book.BookTitle = updateDto.BookTitle;
            book.CategoryId = updateDto.CategoryId;
            book.AuthorId = updateDto.AuthorId;
            book.PublishYear = updateDto.PublishYear;
            book.Publisher = updateDto.Publisher;
            book.Price = updateDto.Price;
            book.Quantity = updateDto.Quantity;
            book.Description = updateDto.Description;
            await _bookRepository.UpdateBookAsync(book);

            if (updateDto.ImageFile != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(updateDto.ImageFile);
                if (uploadResult.Error == null)
                {
                    var oldCover = await _bookRepository.GetCoverImageAsync(id);
                    if (oldCover != null)
                    {
                        await _bookRepository.RemoveBookImageAsync(oldCover);
                    }

                    var newImage = new BookImage
                    {
                        BookId = book.BookId,
                        URL = uploadResult.SecureUrl.ToString(),
                        IsCover = true,
                        OrderIndex = 1
                    };
                    await _bookRepository.AddBookImageAsync(newImage);
                }
            }
            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            return await _bookRepository.DeleteBookAsync(id);
        }
    }
}