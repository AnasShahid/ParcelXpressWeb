using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Enums
{
    public enum InvoiceStatus
    {
        [DisplayName("Due")]
        [StringValue("00001")]
        Due= 1,

        [DisplayName("Paid")]
        [StringValue("00002")]
        Paid = 2,

        [DisplayName("Invalid")]
        [StringValue("00003")]
        Invalid = 3,

    }
}