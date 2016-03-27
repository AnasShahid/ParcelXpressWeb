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
    
    public partial class CUST_DATA
    {
        public CUST_DATA()
        {
            this.CUST_BILL = new HashSet<CUST_BILL>();
            this.JOBS = new HashSet<JOB>();
        }

        public int CustomerId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ContactNo { get; set; }

        public string EmailAddress { get; set; }
        public string AccountRefNumber { get; set; }
        public Nullable<bool> HasAccount { get; set; }
        public Nullable<int> AccountId { get; set; }
    
        public virtual ICollection<CUST_BILL> CUST_BILL { get; set; }
        public virtual ICollection<CUST_CRDT> CUST_CRDT { get; set; }
        public virtual ICollection<JOB> JOBS { get; set; }
    }
}
