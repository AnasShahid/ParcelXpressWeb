﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ParcelXpressConnection : DbContext
    {
        public ParcelXpressConnection()
            : base("name=ParcelXpressConnection")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CUST_DATA> CUST_DATA { get; set; }
        public DbSet<CUST_TRAN> CUST_TRAN { get; set; }
        public DbSet<DRVR_MSGS> DRVR_MSGS { get; set; }
        public DbSet<DRVR_TRAN> DRVR_TRAN { get; set; }
        public DbSet<JOB> JOBS { get; set; }
        public DbSet<JOBS_HTRY> JOBS_HTRY { get; set; }
        public DbSet<JOBS_RESP> JOBS_RESP { get; set; }
        public DbSet<REQT_DRVR> REQT_DRVR { get; set; }
        public DbSet<RESP_CODE> RESP_CODE { get; set; }
        public DbSet<SCRT_QUES> SCRT_QUES { get; set; }
        public DbSet<SESN> SESNs { get; set; }
        public DbSet<STUS_CODE> STUS_CODE { get; set; }
        public DbSet<SYS_USER> SYS_USER { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<TRAN_TYPE> TRAN_TYPE { get; set; }
        public DbSet<CUST_BILL> CUST_BILL { get; set; }
        public DbSet<DRVR_DATA> DRVR_DATA { get; set; }
        public DbSet<EMAL_ACNT> EMAL_ACNT { get; set; }
        public DbSet<CUST_CRDT> CUST_CRDT { get; set; }
        public DbSet<FAQ> FAQS { get; set; }
    }
}