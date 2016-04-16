using ParcelXpress.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace ParcelXpress.Helpers
{
    public static class EmailSender
    {
        public static void SendEmail(string targetEmailAddress,string subject,string body,string fromText="")
        {
            ParcelXpressConnection _db = new ParcelXpressConnection();
            var emailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();
            try {
                MailMessage mm = new MailMessage();
                mm.To.Add(targetEmailAddress);
                mm.Subject = subject;
                mm.Body = body;
                //mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "ParcelXpressInvoice.pdf"));
                mm.IsBodyHtml = true;


                mm.From = new MailAddress(emailAccount.EmailAddress, fromText);


                SmtpClient smtp = GenerateSmtpConfigurations(emailAccount.EmailAddress, emailAccount.Password, emailAccount.EmailClient);

                smtp.Send(mm);
            }
            catch (Exception ex)
            { }
        }
        private static SmtpClient GenerateSmtpConfigurations(string email, string password, int emailClient)
        {
            SmtpClient smtp = new SmtpClient();
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = email;
            NetworkCred.Password = password;

            if (Convert.ToInt16(emailClient) == (int)ParcelXpress.Enums.MailClients.Gmail) //For Gmail
            {
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
            }
            else if (Convert.ToInt16(emailClient) == (int)ParcelXpress.Enums.MailClients.Yahoo)
            {
                smtp.Host = "smtp.mail.yahoo.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
            }

            return smtp;
        }
    }
}