using QLBS.Models;

namespace QLBS.Services.Interfaces
{
    public interface IGhnService
    {
        Task<decimal> CalculateShippingFeeAsync(int toDistrictId, string toWardCode, int totalWeight);
        Task<string?> CreateShippingOrderAsync(OrderTable order, List<OrderDetail> details, int toDistrictId, string toWardCode, int totalWeight, int paymentMethodId);
    }
}