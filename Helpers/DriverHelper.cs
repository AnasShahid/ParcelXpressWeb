﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Models
{
    public partial class DRVR_DATA
    {

        public int ActiveJobsCount { get; set; }

        public int LongDistanceJobCount { get; set; }

        public decimal DriverCommissionAmount { get; set; }

        public bool isTimeIn { get; set; }

        public string AdditionalInfo { get; set; }

    }
}