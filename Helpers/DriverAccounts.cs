using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class DriverAccounts
    {
        public DriverAccounts()
        {
            JobsCount = 0;
            DriverCommission = 0;
            TotalJobPrice = 0;
        }
        public int JobsCount { get; set; }
        public decimal DriverCommission { get; set; }
        public decimal TotalJobPrice { get; set; }
    }
}