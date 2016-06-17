using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Models
{
    public partial class JOB
    {
        private string m_dateString = "";
        public bool longDistanceCheckboxValue { get; set; }
        public string paymentModeDescription { get; set; }

        public string DateString
        {
            get
            {
                if (this.JobDate != null)
                    m_dateString = this.JobDate.AddHours(1).ToString("hh:mm tt");
                return m_dateString;
            }
            set
            {
                m_dateString = value;
            }
        }
    }
}