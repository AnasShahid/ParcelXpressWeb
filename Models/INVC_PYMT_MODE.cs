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
    
    public partial class INVC_PYMT_MODE
    {
        public INVC_PYMT_MODE()
        {
            this.CUST_INVC = new HashSet<CUST_INVC>();
        }
    
        public int PaymentModeId { get; set; }
        public string PaymentModeCode { get; set; }
        public string PaymentModeDsc { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        public virtual ICollection<CUST_INVC> CUST_INVC { get; set; }
    }
}
