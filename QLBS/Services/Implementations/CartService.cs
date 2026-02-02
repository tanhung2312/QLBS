using QLBS.Dtos.Cart;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBookRepository _bookRepository;

        public CartService(
            ICartRepository cartRepository,
            IUserProfileRepository userProfileRepository,
            IBookRepository bookRepository)
        {
            _cartRepository = cartRepository;
            _userProfileRepository = userProfileRepository;
            _bookRepository = bookRepository;
        }

        private async Task<int?> GetUserIdByAccountId(int accountId)
        {
            var profile = await _userProfileRepository.GetByAccountIdAsync(accountId);
            return profile?.UserId;
        }

        public async Task<CartDto?> GetUserCartAsync(int accountId)
        {
            var userId = await GetUserIdByAccountId(accountId);
            if (userId == null) return null;

            var cartItems = await _cartRepository.GetCartByUserIdAsync(userId.Value);

            var itemDtos = cartItems.Select(c => new CartItemDto
            {
                BookId = c.BookId,
                BookTitle = c.Book.BookTitle,
                UnitPrice = c.Book.Price,
                Quantity = c.Quantity,
                StockQuantity = c.Book.Quantity,
                BookImageUrl = c.Book.BookImages?.FirstOrDefault(i => i.IsCover)?.URL
                               ?? c.Book.BookImages?.FirstOrDefault()?.URL
            }).ToList();

            return new CartDto
            {
                Items = itemDtos,
                TotalItems = itemDtos.Sum(i => i.Quantity),
                GrandTotal = itemDtos.Sum(i => i.TotalPrice)
            };
        }

        public async Task<string> AddToCartAsync(int accountId, AddToCartDto dto)
        {
            var userId = await GetUserIdByAccountId(accountId);
            if (userId == null) return "Không tìm thấy thông tin người dùng.";

            var book = await _bookRepository.GetBookByIdAsync(dto.BookId);
            if (book == null || book.IsDeleted) return "Sách không tồn tại.";

            var existingItem = await _cartRepository.GetCartItemAsync(userId.Value, dto.BookId);
            int currentQtyInCart = existingItem?.Quantity ?? 0;
            int newTotalQty = currentQtyInCart + dto.Quantity;

            if (newTotalQty > book.Quantity)
            {
                return $"Số lượng sách trong kho không đủ. (Còn lại: {book.Quantity})";
            }

            if (existingItem != null)
            {
                existingItem.Quantity = newTotalQty;
                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var newItem = new Cart
                {
                    UserId = userId.Value,
                    BookId = dto.BookId,
                    Quantity = dto.Quantity,
                    AddedAt = DateTime.Now
                };
                await _cartRepository.AddToCartAsync(newItem);
            }

            return "Success";
        }

        public async Task<bool> UpdateQuantityAsync(int accountId, int bookId, int quantity)
        {
            var userId = await GetUserIdByAccountId(accountId);
            if (userId == null) return false;

            var item = await _cartRepository.GetCartItemAsync(userId.Value, bookId);
            if (item == null) return false;

            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null || quantity > book.Quantity) return false;

            item.Quantity = quantity;
            await _cartRepository.UpdateCartItemAsync(item);
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int accountId, int bookId)
        {
            var userId = await GetUserIdByAccountId(accountId);
            if (userId == null) return false;

            var item = await _cartRepository.GetCartItemAsync(userId.Value, bookId);
            if (item == null) return false;

            await _cartRepository.DeleteCartItemAsync(item);
            return true;
        }

        public async Task<bool> ClearCartAsync(int accountId)
        {
            var userId = await GetUserIdByAccountId(accountId);
            if (userId == null) return false;

            await _cartRepository.ClearCartAsync(userId.Value);
            return true;
        }
    }
}