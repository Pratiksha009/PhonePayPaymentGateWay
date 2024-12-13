using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrBilling_Phone_Pay.Models
{
    public class PaymentRequestViewModel
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string MobileNo{ get; set; }
    }
}