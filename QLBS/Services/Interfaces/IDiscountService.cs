using QLBS.Dtos.Discount;

namespace QLBS.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountCodeResponseDto>> GetAllDiscountsAsync();
        Task<DiscountCodeResponseDto?> GetDiscountByIdAsync(int id);
        Task<DiscountCodeResponseDto?> CreateDiscountAsync(CreateDiscountCodeDto createDto);
        Task<bool> DeleteDiscountAsync(int id);
    }
}