using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class CustomException : Exception
    {
        public string CustomMessage { get; set; }
        public string ExceptionType { get; set; }


        public void LogExceptionMessage(Exception ex, string additionalData)
        {

            string filePath = "F:/Logs/PXP_Log.txt";
            //if (!Directory.Exists(filepath))
            //{
            //    Directory.CreateDirectory(filepath);

            //}  
            //filePath+=""
            //if (!File.Exists(filePath))
            //{
            //    File.Create(filePath).Dispose();

            //}  
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace + "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(additionalData);
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

    }
}