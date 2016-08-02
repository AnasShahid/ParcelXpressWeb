using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Enums
{
    public enum PaymentModes
    {
        [StringValue("00001")]
        [DisplayName("Cash")]
        Cash = 1,

        [StringValue("00002")]
        [DisplayName("Credit Card")]
        CreditCard = 2,

        [StringValue("00003")]
        [DisplayName("Account")]
        Account=3,

        [StringValue("00004")]
        [DisplayName("Contract")]
        Contract = 4
    }
}