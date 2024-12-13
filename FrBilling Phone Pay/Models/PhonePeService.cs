using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FrBilling_Phone_Pay.Models
{
    public class PhonePeService
    {

        private readonly string BaseUrl = "https://api.phonepe.com/v3"; // Replace with actual API URL
        private readonly string ApiKey = "YOUR_API_KEY";               // Your PhonePe API Key
        private readonly string MerchantId = "YOUR_MERCHANT_ID";       // Your Merchant ID

        public async Task<PhonePeResponse> CreateTransaction(PhonePeRequest request)
        {
            using (var client = new HttpClient())
            {
                var jsonRequest = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Add headers
                client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
                client.DefaultRequestHeaders.Add("X-Merchant-ID", MerchantId);

                // Send POST request
                var response = await client.PostAsync($"{BaseUrl}/payment/initiate", content);

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PhonePeResponse>(responseString);
            }
        }
    }
}