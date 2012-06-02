using System;
using System.Configuration;
using System.Diagnostics;
using System.Web.Mvc;
using PostmarkDotNet;
using Triptitude.Biz.Models;
using Triptitude.Biz.Extensions;

namespace Triptitude.Biz.Services
{
    public class EmailService
    {
        public static bool SendTripUpdate(User user, string subject, string htmlBody)
        {
            PostmarkMessage message = new PostmarkMessage
                                          {
                                              From = "admin@triptitude.com",
                                              Subject = subject,
                                              HtmlBody = htmlBody,
                                              To = user.Email,
                                              Tag = "trip-update"
                                          };

            var response = Send(message);
            return response.Status == PostmarkStatus.Success;
        }

        public static void SentEmailInvite(EmailInvite emailInvite, UrlHelper url)
        {
            PostmarkMessage message = new PostmarkMessage
            {
                From = "admin@triptitude.com",
                To = emailInvite.Email,
                Subject = emailInvite.UserTrip.Trip.Creator.NameOrAnon + " wants to plan a trip with you",
                HtmlBody = string.Format("<a href='{0}'>Click here to start planning.</a>", emailInvite.UserTrip.InvitationURL(url)),
                TextBody = string.Format("Go here to help plan the trip: {0}", emailInvite.UserTrip.InvitationURL(url)),
                Tag = "invite"
            };
            Send(message);
        }

        public static void SentSignupEmail(User user)
        {
            PostmarkMessage message = new PostmarkMessage
            {
                From = "admin@triptitude.com",
                To = user.Email,
                Subject = "Welcome to Triptitude",
                HtmlBody = string.Format("Thanks for joining Triptitude! <a href='{0}'>Click here to sign in.</a>", user.LoginLinkUrl),
                TextBody = string.Format("Thanks for joining Triptitude! Use this link to sign in: {0}", user.LoginLinkUrl),
                Tag = "signup"
            };
            Send(message);
        }

        public static void SendForgotPassEmail(User user)
        {
            PostmarkMessage message = new PostmarkMessage
            {
                From = "admin@triptitude.com",
                To = user.Email,
                Subject = "Triptitude password reset",
                HtmlBody = string.Format("<a href='{0}'>Click here to reset your password.</a>", user.LoginLinkUrl),
                TextBody = string.Format("Use this link to reset your password: {0}", user.LoginLinkUrl),
                Tag = "forgot-pass"
            };
            Send(message);
        }

        public static void SendTripCreated(Trip trip, UrlHelper url)
        {

            PostmarkMessage message = new PostmarkMessage
            {
                From = "admin@triptitude.com",
                To = "mikecomstock@gmail.com",
                Subject = "Trip Created",
                TextBody = string.Format("Trip: {0} User: {1}", url.Details(trip, true), url.Details(trip.Creator, true)),
                Tag = "trip-create"
            };
            Send(message);
        }

        public static void SendUserSignedUp(User user)
        {
            PostmarkMessage message = new PostmarkMessage
            {
                From = "admin@triptitude.com",
                To = "mikecomstock@gmail.com",
                Subject = "User Signed Up",
                TextBody = string.Format("New user signed up! ID: {0} Email: {1}", user.Id, user.Email),
                Tag = "user-signup"
            };
            Send(message);
        }

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
            Send(message);
        }

        public static void SendAdmin(string subject, string body)
        {
            var message = new PostmarkMessage
                              {
                                  From = "admin@triptitude.com",
                                  To = "admin@triptitude.com",
                                  Subject = subject,
                                  HtmlBody = body,
                                  Tag = "error"
                              };
            Send(message);
        }

        private static PostmarkResponse Send(PostmarkMessage message)
        {
            if (!Util.ServerIsProduction)
            {
                message.Subject = message.Subject + " (dev mode)";
                message.HtmlBody = string.Format("<p>Original Recipient: {0}</p>{1}", message.To, message.HtmlBody);
                message.TextBody = "Original Recipient: " + message.To + Environment.NewLine + message.TextBody;
                message.To = "test@triptitude.com";
                message.Cc = null;
                message.Bcc = null;
                message.Tag = "test";
            }

            PostmarkClient client = new PostmarkClient(ConfigurationManager.AppSettings["PostmarkAPIKey"]);
            PostmarkResponse response = client.SendMessage(message);

            Debug.WriteLine(response.Message);
            //TODO: write entire message to database table if failure?
            //if (response.Status != PostmarkStatus.Success) { Console.WriteLine("Response was: " + response.Message); }

            return response;
        }
    }
}