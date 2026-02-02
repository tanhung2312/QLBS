using QLBS.Dtos.Cart;

namespace QLBS.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetUserCartAsync(int accountId);
        Task<string> AddToCartAsync(int accountId, AddToCartDto dto);
        Task<bool> UpdateQuantityAsync(int accountId, int bookId, int quantity);
        Task<bool> RemoveFromCartAsync(int accountId, int bookId);
        Task<bool> ClearCartAsync(int accountId);
    }
}