using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Enums
{
    public enum PaymentModes
    {
        [DisplayName("Cash")]
        Cash = 1,

        [DisplayName("Credit Card")]
        CreditCard = 2,

        [DisplayName("Account")]
        Account=3
    }
}