using QLBS.Dtos.Book;
using QLBS.Dtos.Category;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public CategoryService(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            });
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<CategoryResponseDto?> CreateCategoryAsync(CategoryResponseDto createDto)
        {
            if (await _categoryRepository.ExistsByNameAsync(createDto.CategoryName))
            {
                return null;
            }

            var category = new Category
            {
                CategoryName = createDto.CategoryName,
                Description = createDto.Description
            };

            try
            {
                var newCategory = await _categoryRepository.AddAsync(category);

                return new CategoryResponseDto
                {
                    CategoryId = newCategory.CategoryId,
                    CategoryName = newCategory.CategoryName,
                    Description = newCategory.Description
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;
            category.CategoryName = updateDto.CategoryName;
            category.Description = updateDto.Description;
            return await _categoryRepository.UpdateAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<BookByCategoryResponseDto?> GetBooksByCategoryIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) return null;

            var books = await _bookRepository.GetByCategoryIdAsync(categoryId);

            var bookDtos = books.Select(b => new BookResponseDto
            {
                BookId = b.BookId,
                BookTitle = b.BookTitle,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.CategoryName ?? string.Empty,
                AuthorId = b.AuthorId,
                AuthorName = b.Author?.AuthorName ?? string.Empty,
                PublishYear = b.PublishYear,
                Publisher = b.Publisher,
                Price = b.Price,
                Quantity = b.Quantity,
                Description = b.Description,
                ImageUrl = b.BookImages != null && b.BookImages.Any()
                    ? (b.BookImages.FirstOrDefault(img => img.IsCover)?.URL
                       ?? b.BookImages.OrderBy(img => img.OrderIndex).First().URL)
                    : null
            });

            return new BookByCategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                TotalCount = bookDtos.Count(),
                Books = bookDtos
            };
        }
    }
}