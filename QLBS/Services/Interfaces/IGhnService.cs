using QLBS.Dtos.Ghn;
using QLBS.Models;

namespace QLBS.Services.Interfaces
{
    public interface IGhnService
    {
        Task<List<ProvinceDto>> GetProvincesAsync();
        Task<List<DistrictDto>> GetDistrictsAsync(int provinceId);
        Task<List<WardDto>> GetWardsAsync(int districtId);
        Task<decimal> CalculateShippingFeeAsync(int toDistrictId, string toWardCode, int totalWeight);
        Task<string?> CreateShippingOrderAsync(OrderTable order, List<OrderDetail> details, int districtId, string wardCode, int totalWeight, int paymentMethodId);
    }
}