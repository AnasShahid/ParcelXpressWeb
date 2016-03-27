using System;
using System.Collections.Generic;
using System.Linq;

namespace ParcelXpress.Enums
{
    public enum TransactionTypeCode 
    {
        [DisplayName("In")] //For driver it means his own money against a job
        [StringValue("00001")]
        In=1,

        [DisplayName("Out")]    // For driver it means the money he owes to parcel xpress
        [StringValue("00002")]
        Out = 2,

     

    }
}
