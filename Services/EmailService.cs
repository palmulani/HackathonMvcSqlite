using System.Net;
using System.Net.Mail;

namespace HackathonMvcSqlite.Services
{
    public class EmailService
    {
        public async Task SendOtp(string email, string otp)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("palmulani1@gmail.com", "favv trxi buwh cxhj"),
                EnableSsl = true
            };

            var mail = new MailMessage(
                "palmulani1@gmail.com",
                email,
                "Password Reset OTP",
                $"Your OTP for password reset is: {otp}");

            await smtp.SendMailAsync(mail);
        }
    }
}