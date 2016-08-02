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
using System.Text.RegularExpressions;
using System.Configuration;


namespace ParcelXpress.Helpers
{
    public static class PDFGenerator
    {
        public static string invoiceNumber = "";
        public static string createCustomerReportMarkup(CUST_DATA customerInformation, List<CustomerJobDriver> reportParameters, decimal previousPaidAmount, bool isPaid = false)
        {
            string companyName = "Swiftee Ltd";
            string companyAddress = "Harvest House<br />Leaside Road<br />London, E5 9LU";
            string companyPhone = "020 8800 8090";
            string email = "info@swiftee.com";
            string website = "www.swiftee.com";
            string bankName = "Barclays";
            string accountNumber = "20391727";
            string sortCode = "20-46-60";
            string basePath = ConfigurationManager.AppSettings["ApplicationPath"].ToString();
            string pathToImage = "/Styles/Resources/SwifteeInvoiceLogo.png";
            if (isPaid)
                previousPaidAmount = reportParameters.Sum(p => p.RemainingAmount); // If paid already then set paid amount

            SetInvoiceNumber();
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='margin:25mm'>");
            sb.Append("<div style='font-family:Arial;'>");
            sb.Append("<table width='100%' cellspacing='0' cellpadding='0'>");
            sb.Append("<tr>");
            sb.Append("<td align='left'><img src='" + basePath + pathToImage + "' height='90px' width='130px' /></td>");
            sb.Append("<td align='right' style='font-size:10px'><b>"+ companyName + "</b><br /><div style='font-size:7px'>");
            sb.Append(companyAddress);
            sb.Append("<br />");
            sb.Append(companyPhone);
            sb.Append("<br />");
            sb.Append(email);
            sb.Append("<br />");
            sb.Append(website);
            sb.Append("</div> </td></tr>");
            sb.Append("<tr><td bgcolor='#000000' colspan = '2'><div bgcolor='#000000' style='height:5px;'></div><td></tr>");
            sb.Append("<tr><td colspan = '2'></td></tr>");
            sb.Append("<tr><td align='left' style='font-size:8px' ><b>Bill To:</b><br />");
            sb.Append(customerInformation.CustomerName);
            sb.Append("</td><td align='right'><div style='font-size:11.5px'><b>INVOICE # "+invoiceNumber+"</b></div><div style='font-size:8px'><b>Date: </b>");
            sb.Append(DateTime.Now.ToUniversalTime().ToString("dd/MM/yyyy"));
            sb.Append("</div></td></tr>");
            sb.Append("</table>");
            sb.Append("<br />");
            sb.Append("<table  width='100%' style='font-size:7.5px' cellspacing='0'>");
            sb.Append("<thead bgcolor='#c8c8c8'><tr>");//Column headers
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;'><b>Job Date</b></th>");
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;' colspan = '2'><b>Pickup Address</b></th>");
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;' colspan = '2'><b>Drop Address</b></th>");
            sb.Append("<th bgcolor='#c8c8c8' align='left' style='padding-left:3px;' colspan = '2'><b>Reference</b></th>");
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
                if (item.DropAddress1 != null)
                    sb.Append("<br />" + item.DropAddress1);
                if (item.DropAddress2 != null)
                    sb.Append("<br />" + item.DropAddress2);
                if (item.DropAddress3 != null)
                    sb.Append("<br />" + item.DropAddress3);
                if (item.DropAddress4 != null)
                    sb.Append("<br />" + item.DropAddress4);
                sb.Append("</td><td style='padding-left:3px;' colspan = '2'>");
                sb.Append(item.Reference);
                sb.Append("</td><td style='padding-left:3px;' colspan = '2'>");
                sb.Append(item.ChargeDescription);
                sb.Append("</td><td align='right' style='padding-right:3px;'>");
                sb.Append(item.RemainingAmount.ToString());
                sb.Append("</td></tr>");
            }
            sb.Append("<br />");
            sb.Append("<tr><td colspan = '7'></td><td align='left' style='padding-left:3px;' colspan = '2' bgcolor='#c8c8c8'><b>TOTAL</b></td><td align='right' style='padding-right:3px;' bgcolor='#c8c8c8'><b>" + reportParameters.Sum(p => p.RemainingAmount) + "</b></td></tr>");
           // sb.Append("<tr><td colspan = '7'></td><td align='left' style='padding-left:3px;' colspan = '2'>Amount Paid</td><td align='right' style='padding-right:3px;'>" + previousPaidAmount + "</td></tr>");
           // sb.Append("<tr bgcolor='#c8c8c8' ><td colspan = '7' bgcolor='#c8c8c8'></td><td align='left' style='padding-left:3px;' bgcolor='#c8c8c8' colspan = '2'><b>Balance Due</b></td><td align='right' style='padding-right:3px;' bgcolor='#c8c8c8'><b>" + ((reportParameters.Sum(p => p.RemainingAmount)) - previousPaidAmount) + "</b></td></tr>");


            sb.Append("</table>");
            sb.Append("<br /><table width='100%' cellspacing='10' cellpadding='0'><tr>");
            //if (isPaid)
            //{
            //    sb.Append("<td align='left' style='font-size:14px'><b>This invoice has already been settled.</b></td>");
            //}
            //else
            //{
                sb.Append("<td align='left' style='font-size:10px' ><b>PAYMENT INSTRUCTIONS</b><br /><br />");
                sb.Append("<div style='font-size:8px'>");
                sb.Append("Bank account details:<br />" + "Swiftee" + "<br />");
                sb.Append("Sort Code: " + sortCode + "<br />");
                sb.Append("Account Number: " + accountNumber );
                sb.Append("</div></td>");
            //}
            sb.Append("<td style='font-size:10px;'><b>THANK YOU</b><br /><br /> ");
            sb.Append("<div style='font-size:8px'>Thanks for using Swiftee!<br />Your reliable Courier & Delivery solution<br /><br />We're looking forward to serving you again.</div>");
            sb.Append("</td></tr>");

            sb.Append("</table></div></div>");


            return createAndEmailPdf(sb, customerInformation.EmailAddress);
        }

        private static string createAndEmailPdf(StringBuilder sb, string customerEmailAddress)
        {

            StringBuilder disclaimer = new StringBuilder();
            disclaimer.Append("<br /><br /><br /><i>If you ever have any questions, misunderstandings, please call us as soon as possible so that we can sort it out for in the best way possible.");
            disclaimer.Append("<br />If you are on direct debit - you don't have to do anything, your payment will be sent to us within 4 days hopefully.</i>");                 
            ParcelXpressConnection _db = new ParcelXpressConnection();
            var emailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        StringReader sr = new StringReader(sb.ToString());

                        Document pdfDoc = new Document(PageSize.A4, 16f, 16f, 16f, 8f);
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
                            mm.Subject = "Invoice # "+invoiceNumber+" from Swiftee";
                            mm.Body = "<html><body>Dear Customer,<br /><br />Thanks for your custom.<br />Please find attached the invoice of your job.<br /><br />Thanks again for using SWIFTEE!!<br/>Your Reliable Courier & Delivery Solution.<br /><br />" +disclaimer.ToString()/*+ emailSignature.ToString()*/+ " </html></body>";
                            mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "SwifteeInvoice.pdf"));
                            mm.IsBodyHtml = true;


                            mm.From = new MailAddress("accounts@swiftee.com", "Swiftee");


                            SmtpClient smtp = GenerateSmtpConfigurations(emailAccount.EmailAddress, emailAccount.Password, emailAccount.EmailClient);

                            smtp.Send(mm);
                        }
                    }
                }
                return invoiceNumber;
            }
            catch (Exception ex)
            {
                return null;
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

        private static void SetInvoiceNumber() {
            ParcelXpressConnection _db = new ParcelXpressConnection();
            var lastInvoice = _db.CUST_INVC.OrderByDescending(p => p.InvoiceId);
            if (lastInvoice == null||lastInvoice.FirstOrDefault()==null||lastInvoice.FirstOrDefault().InvoiceNumber==null|| lastInvoice.FirstOrDefault().InvoiceNumber.Equals(""))
                invoiceNumber = "451";  //Starting number given by client
            else
            {
                invoiceNumber = (int.Parse(lastInvoice.FirstOrDefault().InvoiceNumber) + 1).ToString();
            }
        }

    }

}