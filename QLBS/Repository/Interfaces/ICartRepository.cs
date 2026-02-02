using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetCartByUserIdAsync(int userId);
        Task<Cart?> GetCartItemAsync(int userId, int bookId);
        Task AddToCartAsync(Cart cartItem);
        Task UpdateCartItemAsync(Cart cartItem);
        Task DeleteCartItemAsync(Cart cartItem);
        Task ClearCartAsync(int userId);
    }
}