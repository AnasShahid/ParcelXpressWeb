using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ParcelXpress.Models;

namespace ParcelXpress.Helpers
{
    public static class GcmSender
    {
        static string ProjectId = "617448801411";
        static string ServerId = "AIzaSyC9WzmIY_BjYYeykLTk5wiClid9K7jsJ7A";

        static ParcelXpressConnection _db = new ParcelXpressConnection();

        public static async Task<bool> SendToAll(string message, string key)
        {
            try
            {
                ParcelXpressConnection disposableConnection = new ParcelXpressConnection();
                var activeDrivers = disposableConnection.DRVR_DATA.Where(d => d.IsActive == true&&d.IsDeleted!=true);
                foreach (var driver in activeDrivers)
                {
                    await Task.Run(() => GcmSend(driver.GcmId, message, key));
                }
                disposableConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> SendToSingle(DRVR_DATA driver, string message, string key)
        {
            try
            {
                await Task.Run(() => GcmSend(driver.GcmId, message, key));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool SendToSingleSyncronously(DRVR_DATA driver, string message, string key)
        {
            try
            {
                 GcmSend(driver.GcmId, message, key);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private static void GcmSend(string GcmId, string message, string key)
        {
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerId));
            tRequest.Headers.Add(string.Format("Sender: id={0}", ProjectId));

            string postData = "collapse_key=" + key + "&time_to_live=7200&data.message=" + message + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + GcmId + "";

            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;
            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse tResponse = tRequest.GetResponse(); dataStream = tResponse.GetResponseStream();
            StreamReader tReader = new StreamReader(dataStream);
            String sResponseFromServer = tReader.ReadToEnd();  //Get response from GCM server  
            //  label_Result.Text = sResponseFromServer; //Assigning GCM response to Label text
            tReader.Close(); dataStream.Close();
            tResponse.Close();
        }
    }
}