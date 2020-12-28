using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinePay.Models
{
    public class Product
    {
        public string id { get; set; }

        public string name { get; set; }

        public string imageUrl { get; set; }

        public int quantity { get; set; }

        public int price { get; set; }
    }
}