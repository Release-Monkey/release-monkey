using System.Net;
using System.Net.Mail;

namespace ReleaseMonkey.src.Common
{
  public class Email
  {
    public static int BeginRelease = 0;

    public static int AcceptedRelease = 1;

    public static int RejectedRelease = 2;

    public static int WelcomeNewPrimaryTester = 3;

    public static int WelcomeNewBetaTester = 4;

    private static List<String> body = [
        "A new release has been created for <project>, please begin testing. You can download the release to test at <link>.\n\nPlease provide feedback for this release here <home>.\n\nThe <project> Team",
            "The release <release> has been approved",
            "The release <release> has been rejected",
            "Hi,\n\nYou have been added as a primary tester for <project>. Your feedback is highly"
                +" appreciated by the <project> team. We will notify you via this email address of new releases to test.\n\nThe <project> Team",
            "Hi,\n\nYou have been added as a beta tester for <project>. Your feedback is highly"
                +" appreciated by the <project> team. We will notify you via this email address of new releases to test.\n\nThe <project> Team",
        ];
    public static void sendEmail(List<string> toAddresses, string releaseName, string projectName, string downloadLink, int type)
    {
      if (toAddresses.Count == 0)
      {
        return;
      }
      else
      {
        MailAddress from = new MailAddress("releasemonkey01@gmail.com");
        MailMessage email = new MailMessage();
        email.Subject = projectName + ": " + releaseName;
        email.Body = body[type]
          .Replace("<project>", projectName)
          .Replace("<release>", releaseName)
          .Replace("<link>", downloadLink)
          .Replace("<home>", Environment.GetEnvironmentVariable("WEB_HOME"));
        email.From = from;
        foreach (string toAddress in toAddresses)
        {
          email.To.Add(toAddress);
        }

        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp.gmail.com";
        smtp.Port = 587;
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
}
