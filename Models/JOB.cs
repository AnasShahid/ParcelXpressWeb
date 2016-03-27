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
    
    public partial class JOB
    {
        public JOB()
        {
            this.JOBS_HTRY = new HashSet<JOBS_HTRY>();
            this.JOBS_RESP = new HashSet<JOBS_RESP>();
        }

        public int JobId { get; set; }
        public string JobStatus { get; set; }
        [Required]
        public string PickupAddress { get; set; }
        public string DropAddress { get; set; }
        [Required(ErrorMessage = "Amount Field is required")]
        public Nullable<decimal> Price { get; set; }
        [Required]
        public string CustomerPhone { get; set; }
        [Required]
        public string CustomerName { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> DriverId { get; set; }
        public Nullable<decimal> DriverCommission { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> PaymentReceived { get; set; }
        public System.DateTime JobDate { get; set; }
        public Nullable<bool> AccountPaymentInd { get; set; }
        public string ChargesDescription { get; set; }
        public Nullable<bool> LongDistanceInd { get; set; }
        public string DropoffContact { get; set; }
        public string DropAddress1 { get; set; }
        public string DropAddress2 { get; set; }
        public string DropAddress3 { get; set; }
        public string DropAddress4 { get; set; }
        public string TypeOfParcel { get; set; }
    
        public virtual CUST_DATA CUST_DATA { get; set; }
        public virtual ICollection<JOBS_HTRY> JOBS_HTRY { get; set; }
        public virtual ICollection<JOBS_RESP> JOBS_RESP { get; set; }
        public virtual DRVR_DATA DRVR_DATA { get; set; }
    }
}