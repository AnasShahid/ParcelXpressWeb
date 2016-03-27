using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Enums
{
    public enum ResponseCode
    {
        [DisplayName("Less than 5 minutes")]
        [StringValue("00001")]
        LessThanFive= 1,

        [DisplayName("5 to 10 minutes")]
        [StringValue("00002")]
        FiveToTen = 2,

        [DisplayName("10 to 15 minutes")]
        [StringValue("00003")]
        TenToFifteen = 3,

        [DisplayName("15 to 20 minutes")]
        [StringValue("00004")]
        FifteenToTwenty = 4,

        [DisplayName("More than 20 minutes")]
        [StringValue("00005")]
        MoreThanTwenty = 5
    }
}