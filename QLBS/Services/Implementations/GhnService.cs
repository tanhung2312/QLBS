using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QLBS.Constants;
using QLBS.Helpers;
using QLBS.Models;
using QLBS.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace QLBS.Services.Implementations
{
    public class GhnService : IGhnService
    {
        private readonly HttpClient _httpClient;
        private readonly GhnSettings _settings;

        public GhnService(HttpClient httpClient, IOptions<GhnSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.ApiBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Token", _settings.ApiToken);
            _httpClient.DefaultRequestHeaders.Add("ShopId", _settings.ShopId.ToString());
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<decimal> CalculateShippingFeeAsync(int toDistrictId, string toWardCode, int totalWeight)
        {
            var requestData = new
            {
                service_type_id = _settings.ServiceTypeId,
                to_district_id = toDistrictId,
                to_ward_code = toWardCode,
                height = 10,
                length = 10,
                width = 10,
                weight = totalWeight
            };

            var response = await _httpClient.PostAsync("shipping-order/fee",
                new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return 0;

            var content = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(content)!;
            return (decimal)result.data.total;
        }

        public async Task<string?> CreateShippingOrderAsync(OrderTable order, List<OrderDetail> details, int toDistrictId, string toWardCode, int totalWeight, int paymentMethodId)
        {
            int codAmount = paymentMethodId == PaymentMethodConstants.COD ? (int)order.TotalAmount : 0;

            var items = details.Select(d => new {
                name = d.Book.BookTitle,
                quantity = d.Quantity,
                weight = _settings.DefaultWeight
            }).ToList();

            var requestData = new
            {
                payment_type_id = _settings.PaymentTypeId,
                to_name = order.ReceiverName,
                to_phone = order.ReceiverPhone,
                to_address = order.ShippingAddress,
                to_ward_code = toWardCode,
                to_district_id = toDistrictId,
                cod_amount = codAmount,
                weight = totalWeight,
                service_type_id = _settings.ServiceTypeId,
                required_note = "CHOXEMHANGKHONGTHU",
                items = items
            };

            var response = await _httpClient.PostAsync("shipping-order/create",
                new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(content)!;
            return (string)result.data.order_code;
        }
    }
}