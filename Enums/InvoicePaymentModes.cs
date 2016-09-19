using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Enums
{
    public enum InvoicePaymentModes
    {
        [StringValue("00001")]
        [DisplayName("Cash")]
        Cash = 1,

        [StringValue("00002")]
        [DisplayName("Bacs")]
        Bacs = 2,

        [StringValue("00003")]
        [DisplayName("GoCardless")]
        GoCardless = 3
    }
}