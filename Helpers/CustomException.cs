using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParcelXpress.Helpers
{
    public class CustomException:Exception
    {
        public string CustomMessage { get; set; }
        public string ExceptionType { get; set; }


    }
}