using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using QLBS.Dtos.Order;
using QLBS.Helpers;
using QLBS.Services.Interfaces;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendOrderConfirmationAsync(string toEmail, string receiverName, OrderResultDto order)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = $"✅ Xác nhận đơn hàng #{order.OrderId}";

        email.Body = new TextPart("html")
        {
            Text = $"""
            <div style="font-family:Arial,sans-serif; max-width:600px; margin:auto; border:1px solid #e0e0e0; border-radius:8px; padding:24px;">
              <h2 style="color:#2e7d32;">🎉 Đặt hàng thành công!</h2>
              <p>Xin chào <strong>{receiverName}</strong>,</p>
              <p>Đơn hàng của bạn đã được xác nhận với thông tin:</p>
              <table style="width:100%; border-collapse:collapse; margin:16px 0;">
                <tr style="background:#f5f5f5;">
                  <td style="padding:8px 12px; font-weight:bold;">Mã đơn hàng</td>
                  <td style="padding:8px 12px;">#{order.OrderId}</td>
                </tr>
                <tr>
                  <td style="padding:8px 12px; font-weight:bold;">Tổng thanh toán</td>
                  <td style="padding:8px 12px; color:#c62828;"><strong>{order.TotalAmount:N0}đ</strong></td>
                </tr>
                <tr style="background:#f5f5f5;">
                  <td style="padding:8px 12px; font-weight:bold;">Trạng thái</td>
                  <td style="padding:8px 12px;">🚚 Đang xử lý</td>
                </tr>
              </table>
              <p>Chúng tôi sẽ liên hệ khi đơn hàng được giao. Cảm ơn bạn đã mua sắm!</p>
              <hr style="border:none; border-top:1px solid #eee; margin:20px 0;"/>
              <p style="color:#9e9e9e; font-size:12px;">Email này được gửi tự động, vui lòng không trả lời.</p>
            </div>
            """
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}