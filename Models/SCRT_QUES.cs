//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ParcelXpress.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SCRT_QUES
    {
        public SCRT_QUES()
        {
            this.DRVR_DATA = new HashSet<DRVR_DATA>();
            this.SYS_USER = new HashSet<SYS_USER>();
        }
    
        public int QuestionId { get; set; }
        public string SecurityQuestion { get; set; }
    
        public virtual ICollection<DRVR_DATA> DRVR_DATA { get; set; }
        public virtual ICollection<SYS_USER> SYS_USER { get; set; }
    }
}
