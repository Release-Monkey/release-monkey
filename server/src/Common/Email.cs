using System.Net;
using System.Net.Mail;

namespace ReleaseMonkey.src.Common
{
    public class Email
    {
        public static int BeginRelease = 0;
        
        public static int AcceptedRelease = 1;

        public static int RejectedRelease = 2;

        private static List<String> body = ["A new release has been created for <project>, please begin testing. You can download the release to test at <link>.", "The release <release> has been approved", "The release <release> has been rejected"];
        public static void sendEmail(List<string> toAddresses, string releaseName, string projectName, string downloadLink, int type)
        {
            MailAddress from = new MailAddress("releasemonkey01@gmail.com");
            MailMessage email = new MailMessage();
            email.Subject = projectName+": "+releaseName;
            email.Body = body[type].Replace("<project>",projectName).Replace("<release>",releaseName).Replace("<link>", downloadLink);
            email.From = from;
            foreach (string toAddress in toAddresses)
            {
                email.To.Add(toAddress);
            }

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("SMTP_USERNAME"), Environment.GetEnvironmentVariable("SMTP_PASSWORD"));
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(email);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
