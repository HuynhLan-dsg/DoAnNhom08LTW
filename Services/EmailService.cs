    using MailKit;
    using MailKit.Net.Smtp;
    using MimeKit;

    namespace DoAnNhom08LTW.Services
    {
        public class EmailService
        {
            private readonly IConfiguration _configuration;

            public EmailService(
                IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task SendOtpEmail(
                string email,
                string otp)
            {
                var message = new MimeMessage();

                message.From.Add(
                    MailboxAddress.Parse(
                        _configuration["EmailSettings:Email"]));

                message.To.Add(
                    MailboxAddress.Parse(email));

                message.Subject = "Xác thực tài khoản";

                message.Body =
                    new TextPart("plain")
                    {
                        Text = $"Mã OTP của bạn là: {otp}"
                    };

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(
                    "smtp.gmail.com",
                    587,
                    MailKit.Security.SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:Email"],
                    _configuration["EmailSettings:Password"]);

                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);
            }
        }
    }