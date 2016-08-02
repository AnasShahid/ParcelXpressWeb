using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Models
{
    public partial class CUST_DATA
    {
        public decimal CustomerPayable { get; set; }
        public bool HasContractCheckboxValue { get; set; }
        public Nullable<DateTime> LastPaymentDate { get; set; }

    }
}