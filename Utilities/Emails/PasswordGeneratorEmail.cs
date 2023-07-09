using System.Net;
using System.Net.Mail;

namespace AnimeVnInfoBackend.Utilities.Emails
{
    public class PasswordGeneratorEmail
    {
        public PasswordGeneratorEmail()
        {
        }

        public async void SendMail(string address, string password, CancellationToken cancellationToken)
        {
            var configuration = new ConfigurationManager();
            configuration.AddJsonFile("appsettings.json");

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(configuration.GetValue<string>("SMTPEmailConfiguration:RootAddress") ?? "", configuration.GetValue<string>("SMTPEmailConfiguration:Sender"));
            mail.To.Add(new MailAddress(address));
            mail.Body = $"""
                <div style="font-size: 20px;">Mật khẩu mới của bạn là: </div>
                <div style="color: red; font-size: 30px;">{password}</div>
                <div><i>Xin hãy lưu ý đổi mật khẩu ngay để tránh mất cắp</i></div>
            """;
            mail.IsBodyHtml = true;
            mail.Subject = "Yêu cầu mật khẩu mới";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = configuration.GetValue<string>("SMTPEmailConfiguration:Host") ?? "";
            smtp.Port = configuration.GetValue<int>("SMTPEmailConfiguration:Port");
            smtp.EnableSsl = configuration.GetValue<bool>("SMTPEmailConfiguration:EnableSSL");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(configuration.GetValue<string>("SMTPEmailConfiguration:RootAddress"), configuration.GetValue<string>("SMTPEmailConfiguration:Password"));
            smtp.SendCompleted += Smtp_SendCompleted;
            await smtp.SendMailAsync(mail, cancellationToken);
        }

        private void Smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            var obj = (SmtpClient)sender;
            if (obj != null) {
                obj.Dispose();
            }
        }
    }
}
