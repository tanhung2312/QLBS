namespace QLBS.Helpers
{
    public class GhnSettings
    {
        public string ApiBaseUrl { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
        public int ShopId { get; set; }
        public int PaymentTypeId { get; set; }
        public int ServiceTypeId { get; set; }
        public int DefaultWeight { get; set; }
    }
}