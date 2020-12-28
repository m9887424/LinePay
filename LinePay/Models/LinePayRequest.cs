using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinePay.Models
{
    public class LinePayRequest
    {
        public int amount { get; set; }
        public string currency { get; set; }
        public string orderId { get; set; }
        public List<Package> packages { get; set; }
        public Redirecturls redirectUrls { get; set; }
    }
}