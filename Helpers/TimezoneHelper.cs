using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class TimezoneHelper
    {
        static string timeZone = ConfigurationManager.AppSettings["timeZone"].ToString();

        public static DateTime ConvertUTCtoLocal(DateTime utcTime)
        {
            TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);
            return localTime;
        }
    }
}