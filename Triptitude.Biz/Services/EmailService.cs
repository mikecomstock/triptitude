using System;
using System.Configuration;
using System.Diagnostics;
using PostmarkDotNet;

namespace Triptitude.Biz.Services
{
    public class EmailService
    {
        public static void SendTest()
        {
            PostmarkMessage message = new PostmarkMessage
            {
                From = "admin@triptitude.com",
                To = "mikecomstock@gmail.com",
                Subject = "Login Detected on " + DateTime.Now,
                HtmlBody = "HTML Body: " + DateTime.Now,
                TextBody = "Text Body: " + DateTime.Now,
                Tag = "test"
            };

            CleanMessage(message);
            PostmarkClient client = new PostmarkClient(ConfigurationManager.AppSettings["PostmarkAPIKey"]);
            PostmarkResponse response = client.SendMessage(message);
            Debug.WriteLine(response.Message);

            //TODO: write entire message to database table if failure
            //if (response.Status != PostmarkStatus.Success) { Console.WriteLine("Response was: " + response.Message); }
        }

        private static void CleanMessage(PostmarkMessage message)
        {
            if (!Util.ServerIsProduction)
            {
                message.HtmlBody = string.Format("<h1><strong>DEV MODE</strong> Original Recipient: {0}</h1>{1}", message.To, message.HtmlBody);
                message.TextBody = "***** DEV MODE *****" + Environment.NewLine + "Original Recipient: " + message.To + Environment.NewLine + message.TextBody;
                message.To = "test@triptitude.com";
                message.Cc = null;
                message.Bcc = null;
            }
        }
    }
}