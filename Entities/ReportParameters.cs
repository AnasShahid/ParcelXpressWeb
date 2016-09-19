using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class ReportParameters
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string PackageName { get; set; }
        public string PackagePrice { get; set; }
        public int? PackageId { get; set; }
        public int CustomerId{ get; set; }
        public string CustomerName { get; set; }


    }
}