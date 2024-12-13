using FrBilling_Phone_Pay.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FrBilling_Phone_Pay.Controllers
{
    public class PhonePayController : Controller
    {
        // GET: PhonePay
        private readonly PhonePeService _phonePeService = new PhonePeService();
        private string merchantId = "PGTESTPAYUAT";
        private string saltKey = "099eb0cd-02cf-4e2a-8aca-3e6c6aff0399";
        private string saltIndex = "1";
        private string apiEndpoint = "/pg/v1/pay";



        public ActionResult RequestPayment()
        {
            return View(new PhonePeRequest());
        }
        // POST: Redirect to PhonePe
        //[HttpPost]
        //public async ActionResult RedirectToPhonePe(PhonePeRequest request)
        //{
        //    string merchantId = "PGTESTPAYUAT"; // Your Merchant ID
        //    int amountInPaise = (int)(request.Amount * 100); // Convert to paise
        //    string saltKey = "f870265b-5284-45be-8596-67c797a052e3"; // Your Salt Key
        //    int saltIndex = 1;

        //    // Create the request payload
        //    var payload = new
        //    {
        //        merchantId = "MERCHANT",
        //        merchantTransactionId =Guid.NewGuid().ToString(),
        //        merchantUserId = "MUID123",
        //        amount = amountInPaise,
        //        redirectUrl = Url.Action("PhonePayResponse", "PhonePay", null, Request.Url.Scheme),
        //        redirectMode = "REDIRECT",
        //        callbackUrl = Url.Action("PhonePayCallback", "PhonePay", null, Request.Url.Scheme),
        //        mobileNumber = "9999999999",
        //        paymentInstrument = new
        //        {
        //            type = "PAY_PAGE"
        //        }
        //    };

        //    // Convert payload to Base64
        //    string jsonPayload = JsonConvert.SerializeObject(payload);
        //    string base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonPayload));

        //    // Generate X-Verify checksum
        //    string apiEndpoint = "/pg/v1/pay";
        //    string toHash = base64Payload + apiEndpoint + saltKey;

        //    string checksum;
        //    using (SHA256 sha256Hash = SHA256.Create())
        //    {
        //        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(toHash));
        //        checksum = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        //    }
        //    string xVerify = $"{checksum}+###+{saltIndex}";

        //    //// Redirect to PhonePe URL
        //    //string paymentUrl = "https://api-preprod.phonepe.com/apis/pg-sandbox/pg/v1/pay";
        //    //var redirectUrl = $"{paymentUrl}?payload={base64Payload}&x-verify={xVerify}";

        //    //return Redirect(redirectUrl);
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        //        client.DefaultRequestHeaders.Add("X-VERIFY", xVerify);
        //        var requestDate = new Dictionary<string, string> { { "request", base64Payload } };
        //        var content = new StringContent(JsonConvert.SerializeObject(requestDate), Encoding.UTF8, "application/json");
        //        var response = await client.PostAsync("https://api-preprod.phonepe.com/apis/merchant-simulator/pg/V1/pay", content);
        //        var responseContent = response.Content.ReadAsStringAsync().Result;


        //    }
        //}

        public string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }





        [HttpPost]
        public async Task<ActionResult> RedirectToPhonePe(PhonePeRequest request)
        {
            try
            {
                string merchantId = "PGTESTPAYUAT";
                string saltKey = "f870265b-5284-45be-8596-67c797a052e3"; 
                int saltIndex = 1;

                // Convert amount to paise
                int amountInPaise = (int)(request.Amount * 100);

                // Create the payload object
                var phoneRequestModel = new
                {
                    merchantId = merchantId,
                    merchantTransactionId = Guid.NewGuid().ToString(),
                    merchantUserId =  "MUID123", 
                    amount = amountInPaise,
                    redirectUrl = Url.Action("PhonePayResponse", "PhonePay", null, Request.Url.Scheme),
                    redirectMode = "REDIRECT",
                    callbackUrl = Url.Action("PhonePayResponse", "PhonePay", null, Request.Url.Scheme),
                    mobileNumber = request.MobileNo,
                    paymentInstrument = new { type = "PAY_PAGE" }
                };



                // Serialize and encode payload
                var jsonPayload = JsonConvert.SerializeObject(phoneRequestModel);
                var base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonPayload));

                // Generate X-VERIFY checksum
                var apiEndpoint = "/pg/v1/pay";
                var stringToHash = base64Payload + apiEndpoint + saltKey;
                var checksum = ComputeSha256Hash(stringToHash) + $"###{saltIndex}";

                // Return the payload and checksum to the frontend
                return Json(new
                {
                    merchantId = merchantId,
                    base64Payload = base64Payload,
                    xVerify = checksum
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return error response
                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing the request.",
                    error = ex.Message
                });
            }


            //// Generate X-Verify
            //string apiEndpoint = "/pg/v1/pay";
            //string toHash = base64Payload + apiEndpoint + saltKey;
            //string checksum;
            //using (SHA256 sha256Hash = SHA256.Create())
            //{
            //    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            //    checksum = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            //}
            //string xVerify = $"{checksum}###{saltIndex}";

            //// Send request to PhonePe
            //using (var client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Add("X-VERIFY", xVerify);

            //    var content = new StringContent(JsonConvert.SerializeObject(new { request = base64Payload }), Encoding.UTF8, "application/json");
            //    var response = await client.PostAsync("https://api-preprod.phonepe.com/apis/pg-sandbox/pg/v1/pay", content);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseContent = await response.Content.ReadAsStringAsync();
            //        dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

            //        if (jsonResponse.success == true && jsonResponse.data.redirectUrl != null)
            //        {
            //            // Redirect to the payment page
            //            return Redirect(jsonResponse.data.redirectUrl.ToString());
            //        }
            //        else
            //        {
            //            // Redirect to PhonePayResponse with failure status
            //            TempData["ErrorMessage"] = "Payment initiation failed.";
            //            return RedirectToAction("PhonePayResponse");
            //        }
            //    }
            //    else
            //    {
            //        // Redirect to PhonePayResponse with failure status
            //        TempData["ErrorMessage"] = "Error connecting to PhonePe.";
            //        return RedirectToAction("PhonePayResponse");
            //    }
        }
    

    public ActionResult PhonePayResponse()
    {
        var paymentStatus = Request.Form; // Or parse QueryString if redirected with GET.

        if (paymentStatus["success"] == "true")
        {
            ViewBag.Message = "Payment Successful!";
            // Update order status in the database.
        }
        else if (TempData["ErrorMessage"] != null)
        {
            ViewBag.Message = TempData["ErrorMessage"];
        }
        else
        {
            ViewBag.Message = "Payment Failed. Reason: " + paymentStatus["message"];
            // Log the failure for debugging.
        }

        return View();
    }




    public async Task<ActionResult> InitiatePayment(PhonePeRequest req)
        {
            var merchantTransactionId = "MT" + DateTime.Now.Ticks.ToString();
            var merchantUserId = "MUID123"; // Use unique user ID

            var payload = new
            {
                merchantId = "PGTESTPAYUAT",
                merchantTransactionId = "MT7850590068188104",
                merchantUserId = "merchantUserId",
                amount = req.Amount * 100, // Convert to paise
                redirectUrl = Url.Action("PhonePayResponse", "PhonePay", null, Request.Url.Scheme),
                redirectMode = "REDIRECT",
                callbackUrl = Url.Action("PhonePayCallback", "PhonePay", null, Request.Url.Scheme),
                mobileNumber = req.MobileNo,
                paymentInstrument = new { type = "PAY_PAGE" }
            };

            // Convert the payload to a Base64 encoded string
            var base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));

            // Generate checksum (X-VERIFY header)
            var checksum = GenerateChecksum(base64Payload);

            // Send the request
            using (var client = new HttpClient())
            {
                var request = new
                {
                    request = base64Payload
                };

                var jsonRequest = JsonConvert.SerializeObject(request);
                //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json; charset=utf-8");
                //content.Headers.Add("X-VERIFY", checksum);
                //content.Headers.Add("Content-Type", "application/json;");
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                content.Headers.Add("X-VERIFY", checksum);


                var response = await client.PostAsync("https://api-preprod.phonepe.com/apis/pg-sandbox" + apiEndpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                    var redirectUrl = responseObject?.data?.redirectInfo?.url;

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        // Redirect user to PhonePe payment page
                        return Redirect(redirectUrl);
                    }
                }
                else
                {
                    // Redirect to PhonePayResponse with failure status
                    TempData["ErrorMessage"] = "Payment initiation failed.";
                    return RedirectToAction("PhonePayResponse");
                }
            }
           
                // Redirect to PhonePayResponse with failure status
                TempData["ErrorMessage"] = "Error connecting to PhonePe.";
                return RedirectToAction("PhonePayResponse");
            
        }

        private string GenerateChecksum(string base64Payload)
        {
            // Concatenate Base64 Payload, API Endpoint, and Salt Key
            var toHash = base64Payload + apiEndpoint +saltKey;

            // Compute SHA-256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(toHash);
                var hashBytes = sha256.ComputeHash(bytes);
                var hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // Return checksum with salt index
                return hashHex + "###" + saltIndex;
            }
        }



        [HttpPost]
        public JsonResult PhonePayCallback(PhonePayCallBack phonepayresponse)
        {
            string data = phonepayresponse.Response;
            byte[] dataBytes = Convert.FromBase64String(data);
            string json = Encoding.UTF8.GetString(dataBytes);

            JObject jsonObject = JObject.Parse(json);
            bool success = (bool)jsonObject["success"];

            if (success)
            {
                int orderId = Convert.ToInt32(jsonObject["data"]["merchantTransactionId"]);
                double amountInRupees = (double)jsonObject["data"]["amount"] / 100.0;

                // Update the database for successful payment.
                return Json(new { status = "Success", message = "Payment Processed Successfully!" });
            }
            else
            {
                // Handle the failure scenario.
                return Json(new { status = "Failure", message = "Payment Failed!" });
            }
        }

      
        //public ActionResult RedirectToPhonePe(string orderId, decimal amount)
        //{
           

        //    string merchantid = "FRBilling";
        //    int amountInCents = (int)(amount * 100); // Convert to paisa.

        //    var RequestData = new
        //    {
        //        merchantId = merchantid,
        //        merchantTransactionId = orderId,
        //        merchantUserId ="9999999999",
        //        amount = amountInCents,
        //        mobileNumber = "9999999999",
        //        redirectUrl = Url.Action("PhonePayResponse", "Payment", null, Request.Url.Scheme),
        //        callbackUrl = Url.Action("PhonePayCallback", "Payment", null, Request.Url.Scheme),
        //        redirectMode = "POST",
        //        paymentInstrument = new
        //        {
        //            type = "PAY_PAGE"
        //        }
        //    };

        //    string jsonData = JsonConvert.SerializeObject(RequestData);
        //    string base64EncodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonData));
        //    string saltKey = "e144d390-1c3f-4942-910e-5f9f9e0d08e8";
        //    int saltIndex = 2;

        //    string toHash = base64EncodedPayload + "/pg/v1/pay" + saltKey;
        //    string xVerify;

        //    using (var sha256Hash = SHA256.Create())
        //    {
        //        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(toHash));
        //        StringBuilder builder = new StringBuilder();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            builder.Append(bytes[i].ToString("x2"));
        //        }
        //        xVerify = builder.ToString() + "###" + saltIndex;
        //    }

        //    string paymentUrl = "https://api.phonepe.com/pg/v1/pay"; // Replace with the actual URL.
        //    var redirectUrl = $"{paymentUrl}?payload={base64EncodedPayload}&x-verify={xVerify}";

        //    return Redirect(redirectUrl);
        //}

      
        //[HttpPost]
        //public ActionResult Webhook(PhonePeResponse response)
        //{
        //    // Process the payment status here
        //    return new HttpStatusCodeResult(200);
        //}

        //[HttpPost]
        //public JsonResult GeneratePhonePay( PhonePeRequest request)
        //{
        //    var paymentData = generatePhonePay(request);
        //    if (paymentData.status == "Success")
        //    {
        //        return Json(new { status = "Success", base64EncodedPayload = paymentData.base64EncodedPayload });
        //    }
        //    else
        //    {
        //        return Json(new { status = "Failure", message = "Payment initiation failed." });
        //    }
        //}

    }
}