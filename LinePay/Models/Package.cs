using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinePay.Models
{
    public class Package
    {
        public string id { get; set; }

        public int amount { get; set; }

        public string name { get; set; }

        public List<Product> products { get; set; }
    }
}