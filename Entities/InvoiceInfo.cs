using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class InvoiceInfo
    {
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string Amount { get; set; }
        public string Paid { get; set; }
        public string PaidDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentMode { get; set; }


    }
}