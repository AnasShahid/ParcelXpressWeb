using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class Balance
    {
        public Balance()
        {
            DriverBalance = 0;
            PxpBalance = 0;
        }
        public decimal DriverBalance { get; set; }
        public decimal PxpBalance { get; set; }

    }
}