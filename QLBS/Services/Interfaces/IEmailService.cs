using QLBS.Dtos.Order;

namespace QLBS.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string toEmail, string receiverName, OrderResultDto order);
    }
}
