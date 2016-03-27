﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class CustomerJobDriver
    {
        public int JobId { get; set; }
        public DateTime JobDate { get; set; }
        public string DriverName { get; set; }
     
        public decimal Price { get; set; }

        public decimal ReceivedAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public string PickupAddress { get; set; }
        public string DropAddress { get; set; }
        public string ChargeDescription { get; set; }
        

    }
}