using System.Net;
using System.Net.Mail;

namespace AnimeVnInfoBackend.Utilities.Emails
{
    public class TwoFactorCodeEmail
    {
        public async void SendMail(string address, string code, CancellationToken cancellationToken)
        {
            var configuration = new ConfigurationManager();
            configuration.AddJsonFile("appsettings.json");

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(configuration.GetValue<string>("SMTPEmailConfiguration:RootAddress") ?? "", configuration.GetValue<string>("SMTPEmailConfiguration:Sender"));
            mail.To.Add(new MailAddress(address));
            mail.Body = $"""
                <div style="font-size: 20px;">Mã xác nhận của bạn là: </div>
                <div style="color: red; font-size: 30px;">{code}</div>
                <div><i>(Mã sẽ hết hạn trong vòng 5 phút nữa)</i></div>
            """;
            mail.IsBodyHtml = true;
            mail.Subject = "Mã xác nhận đăng nhập";

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
            if(obj != null) {
                obj.Dispose();
            }
        }

        public TwoFactorCodeEmail()
        {

        }
    }
}
