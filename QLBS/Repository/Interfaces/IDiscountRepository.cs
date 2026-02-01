using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<DiscountCode>> GetAllAsync();
        Task<DiscountCode?> GetByIdAsync(int id);
        Task<DiscountCode?> GetByCodeAsync(string code);
        Task<DiscountCode> AddAsync(DiscountCode discountCode);
        Task<bool> DeleteAsync(int id);
        Task<bool> CodeExistsAsync(string code);
    }
}