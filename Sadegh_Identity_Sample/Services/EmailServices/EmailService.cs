using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Unicode;

namespace Sadegh_Identity_Sample.Services.EmailServices
{
    public class EmailService
    {
        public Task Execute(string email , string body , string subject)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "Smtp.Gmail.com";
            client.EnableSsl = true;
            client.DeliveryMethod= SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("sadeghemailsender@gmail.com", "ztoeantntxpdeamz");
            MailMessage message = new MailMessage("sadeghemailsender@gmail.com" , email , subject , body);
            message.IsBodyHtml = true;
            message.BodyEncoding = UTF8Encoding.UTF8;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            client.Send(message);
            return Task.CompletedTask;
        }
    }
}
