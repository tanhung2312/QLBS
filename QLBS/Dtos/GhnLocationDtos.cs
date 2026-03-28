namespace QLBS.Dtos.Ghn
{
    public class ProvinceDto
    {
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = "";
    }

    public class DistrictDto
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = "";
    }

    public class WardDto
    {
        public string WardCode { get; set; } = "";
        public string WardName { get; set; } = "";
    }

    public class ShippingFeeDto
    {
        public decimal ShippingFee { get; set; }
    }
}