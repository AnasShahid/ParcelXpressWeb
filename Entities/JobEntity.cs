using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParcelXpress.Models;

namespace ParcelXpress.Entities
{
    public class JobEntity
    {
        private JOB m_job = null;
        private CUST_DATA m_custData = null;


        public JOB JOB
        {
            get { return m_job; }
            set { m_job = value; }
        }

        public CUST_DATA CUST_DATA
        {
            get { return m_custData; }
            set { m_custData = value; }
        }

    }
}