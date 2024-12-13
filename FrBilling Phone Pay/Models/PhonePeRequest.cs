using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrBilling_Phone_Pay.Models
{
    public class PhonePeRequest
    {
        public string MerchantId { get; set; }
        public string TransactionId { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Currency { get; set; }
        public string SuccessUrl { get; set; }
        public string FailureUrl { get; set; }
        
        public string MobileNo { get; set; }
        public string CustomerName { get; set; }
    
    
    }

  


    public class PhonePeResponse
    {
        public string Success { get; set; }
        public string Message { get; set; }
        public string MerchantTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentState { get; set; }
    }

    public class PhonePayCallBack
    {
        public string Response { get; set; }
    }

}