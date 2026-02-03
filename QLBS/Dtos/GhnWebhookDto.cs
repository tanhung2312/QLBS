namespace QLBS.Dtos.Ghn
{
    public class GhnWebhookDto
    {
        public string OrderCode { get; set; } = string.Empty; // Mã đơn GHN
        public string Status { get; set; } = string.Empty;    // delivered, return, ...
        public DateTime Time { get; set; }
    }
}