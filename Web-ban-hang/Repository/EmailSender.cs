using System.Net.Mail;
using System.Net;

namespace Web_ban_hang.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587) // cổng 465, khoong bảo mật bằng 587
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("haidangftest@gmail.com", "xmqggvixbxeveoix")
            };

            return client.SendMailAsync(
                new MailMessage(from: "haidangftest@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
