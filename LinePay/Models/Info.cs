using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinePay.Models
{
    public class Info
    {
        public Paymenturl paymentUrl { get; set; }

        public long transactionId { get; set; }

        public string paymentAccessToken { get; set; }
    }
}