using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ParcelXpress.Helpers
{
    public class AddChagesViewModel
    {
        public int JobId { get; set; }
        [Required(ErrorMessage = "Amount Field is required")]
        public Nullable<decimal> Price { get; set; }
        [Required]
        public string ChargesDescription { get; set; }
    }
}