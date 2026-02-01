using QLBS.Dtos.Category;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
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
    }
}