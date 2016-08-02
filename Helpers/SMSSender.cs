using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Threading.Tasks;
using ParcelXpress.Models;
using System.Net;
using System.Configuration;
using ParcelXpress.Enums;
using ParcelXpress.Models;

namespace ParcelXpress.Helpers
{
    public static class SMSSender
    {
        public static async Task<bool> SendSmsToCustomer(string phoneNumber, int status, string replaceText = "", int? customerId = null)
        {
           
            try
            {
                phoneNumber = phoneNumber.Replace(" ", String.Empty);
                if (phoneNumber.StartsWith("020"))  //Dont send sms to landline numbers
                    return true;
                if (customerId != null && customerId > 0)
                {
                    using (ParcelXpressConnection _db = new ParcelXpressConnection())
                    {
                        if (_db.CUST_DATA.Find(customerId).RecieveNotifications == false)
                            return true;
                    }
                }
                string messageText = "";
                if (status == (int)StatusCode.Assigned)
                {
                    messageText = ConfigurationManager.AppSettings["PickedUpSms"].ToString();
                    messageText = messageText.Replace("----", replaceText);
                }
                else if (status == (int)StatusCode.Closed)
                    messageText = ConfigurationManager.AppSettings["DroppedOffSms"].ToString();
                await Task.Run(() => SendSMSViaTextLocal(phoneNumber, messageText));
                return true;
            }
            catch (Exception ex)
            {
                new CustomException().SaveExceptionToDB(ex, "");
                return false;
            }
        }

        private static void SendSMSViaTextLocal(string number, string messageText)
        {
            try
            {
                string SenderName = ConfigurationManager.AppSettings["SmsSenderName"].ToString() == null ? "Parcel Express" : ConfigurationManager.AppSettings["SmsSenderName"].ToString();
                string APIKey = ConfigurationManager.AppSettings["SmsApiKey"].ToString();
                if (APIKey == null || APIKey.Trim().Equals(""))
                    throw new Exception("API key not found");
                String message = HttpUtility.UrlEncode(messageText);
                using (var wb = new WebClient())
                {
                    byte[] response = wb.UploadValues("http://api.txtlocal.com/send/", new NameValueCollection()
                {
                {"apiKey" , APIKey},
                //{"hash" , "Your API hash"},
                {"numbers" , number},
                {"message" , message},
                {"sender" , SenderName}
                });
                }
            }
            catch (Exception ex)
            {
                new CustomException().SaveExceptionToDB(ex, "");
            }
        }
    }
}