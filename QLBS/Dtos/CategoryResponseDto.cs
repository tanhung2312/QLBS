using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos.Category
{
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
    public class UpdateCategoryDto : CategoryResponseDto {}
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Tên thể loại không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên thể loại tối đa 100 ký tự")]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}