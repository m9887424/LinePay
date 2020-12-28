using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinePay.Models
{
    public class LinePayResponse
    {
        public string returnCode { get; set; }

        public string returnMessage { get; set; }

        public Info info { get; set; }
    }
}