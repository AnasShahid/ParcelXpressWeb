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
    using System.ComponentModel.DataAnnotations;
    
    public partial class DALY_PRCL_MSTR
    {
        public DALY_PRCL_MSTR()
        {
            this.DALY_PRCL = new HashSet<DALY_PRCL>();
        }
    
        public int DailyParcelMasterId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        [Required]
        public string DropAddress { get; set; }
        [Required(ErrorMessage = "Amount Field is required")]
        public Nullable<decimal> Amount { get; set; }
    
        public virtual CUST_DATA CUST_DATA { get; set; }
        public virtual ICollection<DALY_PRCL> DALY_PRCL { get; set; }
    }
}