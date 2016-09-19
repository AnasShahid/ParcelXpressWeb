using System;
using System.Collections.Generic;
using System.Linq;

namespace ParcelXpress.Enums
{
    public enum StatusCode 
    {
        [DisplayName("Open")]
        [StringValue("00000")]
        Open=1,

        [DisplayName("Picked up")]
        [StringValue("00001")]
        PickedUp = 2,

        [DisplayName("Dropped off")]
        [StringValue("00002")]
        DroppedOff = 3,

        [DisplayName("Payment received")]
        [StringValue("00003")]
        PaymentReceived = 4,

        [DisplayName("Assigned")]
        [StringValue("00004")]
        Assigned = 5,

        [DisplayName("Closed")]
        [StringValue("00005")]
        Closed = 6,

        [DisplayName("Pending")]
        [StringValue("00006")]
        Pending = 7,

        [DisplayName("Sent")]
        [StringValue("00007")]
        Sent = 8,



    }
}
