using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Threading.Tasks;

namespace MusicComposer.Web.BL
{
    public class SendEmail
    {
        public static bool IsSendEmailAvailable(IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.GetValue<string>("MusicComposerEmail:SendGridKey")))
                return false;
            return true;
        }
        public static async Task<bool> Send(IConfiguration configuration, string message)
        {
            string emailTo = configuration.GetValue<string>("MusicComposerEmail:EmailTo");
            string emailFrom = configuration.GetValue<string>("MusicComposerEmail:EmailFrom");
            string sendGridKey = configuration.GetValue<string>("MusicComposerEmail:SendGridKey");
            SendGridClient client = new SendGrid.SendGridClient(sendGridKey);
            EmailAddress from = new EmailAddress(emailFrom, "web page user");
            string subject = "Music Composer Website";
            EmailAddress to = new EmailAddress(emailTo, "Support");
            string plainTextContent = message;
            string htmlContent = message;
            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            Response response = await client.SendEmailAsync(msg);
            if (response.StatusCode == HttpStatusCode.Accepted)
                return true;
            return false;

        }
    }
}
