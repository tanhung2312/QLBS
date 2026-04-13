using Microsoft.Extensions.Options;
using QLBS.Constants;
using QLBS.Dtos.Ghn;
using QLBS.Helpers;
using QLBS.Models;
using QLBS.Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace QLBS.Services.Implementations
{
    public class GhnService : IGhnService
    {
        private readonly HttpClient _httpClient;
        private readonly GhnSettings _settings;
        private readonly string BASE;



        private static readonly System.Text.Json.JsonSerializerOptions JsonOpts =
            new() { PropertyNameCaseInsensitive = true };

        public GhnService(HttpClient httpClient, IOptions<GhnSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            BASE = _settings.ApiBaseUrl.TrimEnd('/');

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        private HttpRequestMessage Req(HttpMethod method, string url)
        {
            var req = new HttpRequestMessage(method, url);
            req.Headers.Add("Token", _settings.ApiToken);
            if (_settings.ShopId > 0)
                req.Headers.Add("ShopId", _settings.ShopId.ToString());
            return req;
        }

        public async Task<List<ProvinceDto>> GetProvincesAsync()
        {
            var response = await _httpClient.SendAsync(
                Req(HttpMethod.Get, $"{BASE}/master-data/province"));

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GHN] GetProvinces {(int)response.StatusCode}: " +
                    await response.Content.ReadAsStringAsync());
                return new();
            }

            var result = await response.Content
                .ReadFromJsonAsync<GhnResponse<List<GhnProvince>>>(JsonOpts);

            return result?.Data?
                .Where(p => !string.IsNullOrEmpty(p.ProvinceName))
                .Select(p => new ProvinceDto
                {
                    ProvinceId = p.ProvinceID,
                    ProvinceName = p.ProvinceName
                })
                .OrderBy(p => p.ProvinceName)
                .ToList() ?? new();
        }

        public async Task<List<DistrictDto>> GetDistrictsAsync(int provinceId)
        {
            var response = await _httpClient.SendAsync(
                Req(HttpMethod.Get,
                    $"{BASE}/master-data/district?province_id={provinceId}"));

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GHN] GetDistricts {(int)response.StatusCode}: " +
                    await response.Content.ReadAsStringAsync());
                return new();
            }

            var result = await response.Content
                .ReadFromJsonAsync<GhnResponse<List<GhnDistrict>>>(JsonOpts);

            return result?.Data?
                .Where(d => !string.IsNullOrEmpty(d.DistrictName))
                .Select(d => new DistrictDto
                {
                    DistrictId = d.DistrictID,
                    DistrictName = d.DistrictName
                })
                .OrderBy(d => d.DistrictName)
                .ToList() ?? new();
        }


        public async Task<List<WardDto>> GetWardsAsync(int districtId)
        {
            var req = Req(HttpMethod.Post, $"{BASE}/master-data/ward");
            req.Content = JsonContent.Create(new { district_id = districtId });

            var response = await _httpClient.SendAsync(req);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GHN] GetWards {(int)response.StatusCode}: " +
                    await response.Content.ReadAsStringAsync());
                return new();
            }

            var result = await response.Content
                .ReadFromJsonAsync<GhnResponse<List<GhnWard>>>(JsonOpts);

            return result?.Data?
                .Where(w => !string.IsNullOrEmpty(w.WardCode))
                .Select(w => new WardDto
                {
                    WardCode = w.WardCode,
                    WardName = w.WardName
                })
                .OrderBy(w => w.WardName)
                .ToList() ?? new();
        }


        public async Task<decimal> CalculateShippingFeeAsync(
            int toDistrictId, string toWardCode, int totalWeight)
        {

            if (_settings.FromDistrictId <= 0)
            {
                Console.WriteLine("[GHN] CẢNH BÁO: FromDistrictId chưa được cấu hình! " +
                    "Vào appsettings.json thêm 'FromDistrictId' = DistrictId của quận shop. " +
                    "Cách lấy: mở cart → chọn tỉnh shop → xem Network response districts → " +
                    "tìm tên quận shop → lấy districtId.");
                return 0;
            }


            var safeWeight = Math.Max(10, Math.Min(totalWeight, 30000));
            Console.WriteLine($"[GHN] CalculateFee request: from={_settings.FromDistrictId} to={toDistrictId} ward={toWardCode} weight={safeWeight} service={_settings.ServiceTypeId}");

            var req = Req(HttpMethod.Post, $"{BASE}/v2/shipping-order/fee");
            req.Content = JsonContent.Create(new
            {
                service_type_id = _settings.ServiceTypeId,
                from_district_id = _settings.FromDistrictId, 
                to_district_id = toDistrictId,
                to_ward_code = toWardCode,
                weight = safeWeight,               
                length = 20,                      
                width = 15,                     
                height = 10                       
            });

            var response = await _httpClient.SendAsync(req);

            if (!response.IsSuccessStatusCode)
            {
                var errBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(
                    $"[GHN] CalculateFee FAILED {(int)response.StatusCode}: {errBody}");
                return 0;
            }

            var result = await response.Content
                .ReadFromJsonAsync<GhnResponse<GhnFeeData>>(JsonOpts);

            var fee = result?.Data?.Total ?? 0;
            Console.WriteLine($"[GHN] CalculateFee OK: {fee:N0}đ " +
                $"(district {_settings.FromDistrictId} → {toDistrictId}, ward {toWardCode})");
            return fee;
        }

        public async Task<string?> CreateShippingOrderAsync(
     OrderTable order, List<OrderDetail> details,
     int districtId, string wardCode,
     int totalWeight, int paymentMethodId)
        {

            int paymentTypeId = paymentMethodId == PaymentMethodConstants.COD ? 1 : 2;


            int codAmount = paymentMethodId == PaymentMethodConstants.COD
                ? (int)(order.TotalAmount - order.ShippingFee) : 0;

            var items = details.Select(d => new {
                name = d.Book?.BookTitle ?? "Sách",
                quantity = d.Quantity,
                weight = _settings.DefaultWeight  
            }).ToList();

            var req = Req(HttpMethod.Post, $"{BASE}/v2/shipping-order/create");
            req.Content = JsonContent.Create(new
            {
                payment_type_id = paymentTypeId,  
                to_name = order.ReceiverName,
                to_phone = order.ReceiverPhone,
                to_address = order.ShippingAddress,
                to_ward_code = wardCode,
                to_district_id = districtId,
                cod_amount = codAmount,           
                weight = totalWeight,
                length = 20,
                width = 15,
                height = 10,
                service_type_id = _settings.ServiceTypeId,
                required_note = "CHOXEMHANGKHONGTHU",
                items = items
            });

            var response = await _httpClient.SendAsync(req);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GHN] CreateOrder FAILED {(int)response.StatusCode}: " +
                    await response.Content.ReadAsStringAsync());
                return null;
            }

            var result = await response.Content
                .ReadFromJsonAsync<GhnResponse<GhnCreateOrderData>>(JsonOpts);

            return result?.Data?.OrderCode;
        }

        private class GhnResponse<T>
        {
            [JsonPropertyName("code")] public int Code { get; set; }
            [JsonPropertyName("message")] public string Message { get; set; } = "";
            [JsonPropertyName("data")] public T? Data { get; set; }
        }

        private class GhnProvince
        {
            [JsonPropertyName("ProvinceID")] public int ProvinceID { get; set; }
            [JsonPropertyName("ProvinceName")] public string ProvinceName { get; set; } = "";
        }

        private class GhnDistrict
        {
            [JsonPropertyName("DistrictID")] public int DistrictID { get; set; }
            [JsonPropertyName("DistrictName")] public string DistrictName { get; set; } = "";
        }

        private class GhnWard
        {
            [JsonPropertyName("WardCode")] public string WardCode { get; set; } = "";
            [JsonPropertyName("WardName")] public string WardName { get; set; } = "";
        }

        private class GhnFeeData
        {
            [JsonPropertyName("total")] public decimal Total { get; set; }
        }

        private class GhnCreateOrderData
        {
            [JsonPropertyName("order_code")] public string OrderCode { get; set; } = "";
        }
    }
}