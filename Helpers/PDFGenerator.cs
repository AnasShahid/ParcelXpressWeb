using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParcelXpress.Models;
using System.Text;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Net;


namespace ParcelXpress.Helpers
{
    public static class PDFGenerator
    {
        public static bool createCustomerReportMarkup(CUST_DATA customerInformation, List<CustomerJobDriver> reportParameters,decimal previousPaidAmount)
        {

            string companyName = "Parcel Xpress";
            string companyAddress = "61 Heathland Rd, London N16 5PQ";
            string companyPhone = "020 8800 8090";
            string email = "info@pxpuk.com";
            string bankName = "Barclays";
            string accountNumber = "60237434";
            string sortCode = "20-46-60";

            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='font-family:Arial'>");
            sb.Append("<table width='100%' cellspacing='0' cellpadding='0'>");
            sb.Append("<tr><td align='right' style='font-size:13px' colspan = '2'><b>PXPUK LTD</b><br /><div style='font-size:8.5px'>");
            sb.Append(companyAddress);
            sb.Append("<br />");
            sb.Append(companyPhone);
            sb.Append("<br />");
            sb.Append(email);
            sb.Append("</div> </td></tr>");
            sb.Append("<tr><td bgcolor='#000000' colspan = '2'><div bgcolor='#000000' style='height:2px;'></div><td></tr>");
            sb.Append("<tr><td colspan = '2'></td></tr>");
            sb.Append("<tr><td align='left' style='font-size:10px' ><b>Bill To:</b>");
            sb.Append(customerInformation.CustomerName);
            sb.Append("</td><td align='right'><div style='font-size:13px'><b>INVOICE</b></div><div style='font-size:10px'><b>Date: </b>");
            sb.Append(DateTime.Now.ToUniversalTime().ToString("dd/MM/yyyy"));
            sb.Append("</div></td></tr>");
            sb.Append("<tr><td colspan = '2'style='font-size:10px'><b>Total Payable:</b> ");
            sb.Append((reportParameters.Sum(p => p.RemainingAmount))-previousPaidAmount);
            sb.Append("</td></tr>");
            sb.Append("</table>");
            sb.Append("<br />");
            sb.Append("<table  width='100%' style='font-size:8.5px' cellspacing='0'>");
            sb.Append("<thead bgcolor='#c8c8c8'><tr>");//Column headers
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;'><b>Job Date</b></th>");
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;' colspan = '2'><b>Pickup Address</b></th>");
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;' colspan = '2'><b>Drop Address</b></th>");
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;' colspan = '2'><b>Description</b></th>");
            sb.Append("<th bgcolor='#c8c8c8' align='right' style='padding-right:3px;'><b>Amount</b></th>");
            sb.Append("</tr></thead>");
            //Now for each for description
            foreach (var item in reportParameters)
            {
                sb.Append("<tr>");
                sb.Append("<td style='padding-left:3px;'>");
                sb.Append(item.JobDate.ToString("dd/MM/yyyy"));
                sb.Append("</td><td style='padding-left:3px;' colspan = '2'>");
                sb.Append(item.PickupAddress);
                sb.Append("</td><td style='padding-left:3px;' colspan = '2'>");
                sb.Append(item.DropAddress);
                sb.Append("</td><td style='padding-left:3px;' colspan = '2'>");
                sb.Append(item.ChargeDescription);
                sb.Append("</td><td align='right' style='padding-right:3px;'>");
                sb.Append(item.RemainingAmount.ToString());
                sb.Append("</td></tr>");
            }
            sb.Append("<br />");
            sb.Append("<tr><td colspan = '5'></td><td align='left' style='padding-left:3px;' colspan = '2' bgcolor='#c8c8c8'><b>TOTAL</b></td><td align='right' style='padding-right:3px;' bgcolor='#c8c8c8'><b>" +reportParameters.Sum(p => p.RemainingAmount) + "</b></td></tr>");
            sb.Append("<tr><td colspan = '5'></td><td align='left' style='padding-left:3px;' colspan = '2'>Amount Paid</td><td align='right' style='padding-right:3px;'>" + previousPaidAmount + "</td></tr>");
            sb.Append("<tr bgcolor='#c8c8c8' ><td colspan = '5' bgcolor='#c8c8c8'></td><td align='left' style='padding-left:3px;' bgcolor='#c8c8c8' colspan = '2'><b>Balance Due</b></td><td align='right' style='padding-right:3px;' bgcolor='#c8c8c8'><b>" + ((reportParameters.Sum(p => p.RemainingAmount)) - previousPaidAmount) + "</b></td></tr>");


            sb.Append("</table>");
            sb.Append("<br /><table width='100%' cellspacing='10' cellpadding='0'>");
            sb.Append("<tr><td align='left' style='font-size:13px' ><b>PAYMENT INSTRUCTIONS:</b><br /><div style='font-size:10px'>Bank Transfer:");
            sb.Append("<br />" + bankName + "<br />");
            sb.Append("Account Number: " + accountNumber + "<br />");
            sb.Append("Sort Code: " + sortCode);
            sb.Append("</div></td><td style='font-size:13px'><b>THANK YOU</b><br /> ");
            sb.Append("<div style='font-size:10px'>Thank you for using Parcel Express, your reliable courier solution.</div>");
            sb.Append("</td></tr></table>");
            sb.Append("</div>");


            return createAndEmailPdf(sb, customerInformation.EmailAddress);
        }

        private static bool createAndEmailPdf(StringBuilder sb, string customerEmailAddress)
        {
            ParcelXpressConnection _db = new ParcelXpressConnection();
            var emailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();            
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        StringReader sr = new StringReader(sb.ToString());

                        Document pdfDoc = new Document(PageSize.A4, 4f, 4f, 8f, 2f);
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            pdfDoc.Open();
                            htmlparser.Parse(sr);
                            //using (TextReader reader = new StringReader(sb.ToString()))
                            //{
                            //    //Parse the HTML and write it to the document
                            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, reader);
                            //}
                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();

                            //SimpleEmailGodaddy();
                            MailMessage mm = new MailMessage();
                            mm.To.Add(customerEmailAddress);
                            mm.Subject = "Parcel Xpress Invoice";
                            mm.Body = "Dear Customer,<br />Please find attached the invoice of your bill. Thank you for using Parcel Xpress.";
                            mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "ParcelXpressInvoice.pdf"));
                            mm.IsBodyHtml = true;


                            mm.From = new MailAddress("info@pxpuk.com", "Parcel Xpress");


                            SmtpClient smtp = GenerateSmtpConfigurations(emailAccount.EmailAddress, emailAccount.Password, emailAccount.EmailClient);

                            smtp.Send(mm);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
        private static void SimpleEmailGodaddy()
        {
            using (SmtpClient client = new SmtpClient())
            {
                MailMessage mm = new MailMessage();
                mm.To.Add("anasshahid88@gmail.com");
                mm.From = new MailAddress("info@pxpuk.com", "Parcel Xpress");
                mm.Subject = "Test Email Message";
                mm.Body = "Test Email message from go Daddy server";


                client.Host = "smtpout.secureserver.net";
                //client.Port = 25;

                client.UseDefaultCredentials = false;
                NetworkCredential credentials = new NetworkCredential("info@pxpuk.com", "@Pini6087");
                client.Credentials = credentials;

                client.Send(mm);
            }
        }
    }
}