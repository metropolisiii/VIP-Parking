using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace VIP_Parking.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(string subject, string body, List<string> recipients)
        {
            var message = new MailMessage();
            foreach (string r in recipients)
            {
                message.To.Add(new MailAddress(r));
            }
            message.From = new MailAddress("jasontest1325@gmail.com");
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("jasontest1325@gmail.com", "Dream333");
                client.Send(message);
            }
        }
        public static void SendEmail(string subject, string body, string recipient)
        {
            SendEmail(subject, body, new List<string> { recipient });
        }
    }
}