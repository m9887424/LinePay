using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using LinePay.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;


namespace LinePay.Controllers
{
    public class LinePayController : Controller
    {
        // GET: LinePay
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RequestLinePay()
        {
            using (var httpClient = new HttpClient())
            {
                var channelId = WebConfigurationManager.AppSettings["channelId"];
                var channelSecret = WebConfigurationManager.AppSettings["channelSecret"];
                var baseUrl = "https://sandbox-api-pay.line.me";
                var apiUrl = "/v3/payments/request";
                string orderId = Guid.NewGuid().ToString();

                // 設定 Body
                var requestBody = new LinePayRequest()
                {
                    amount = 400,
                    currency = "TWD",
                    orderId = Guid.NewGuid().ToString(),
                    redirectUrls = new Redirecturls()
                    {
                        confirmUrl = "http://localhost:59600/LinePay/confirmLinePay"
                        //付款完成後，callback的URL
                    },
                    packages = new List<Package>()
                    {
                        new Package()
                        {
                            id = "pack_1",
                            name = "過年大紅包",
                            amount = 400,
                            products = new List<Product>()
                            {
                                new Product()
                                {
                                    id = "prod_1",
                                    name = "過年大紅包",
                                    imageUrl = "https://picsum.photos/200",
                                    quantity = 1,
                                    price = 400
                                }
                            }
                        }
                    }
                };
                var body = JsonConvert.SerializeObject(requestBody);
                string signature = HashLinePayRequest(channelSecret, apiUrl, body, orderId, channelSecret);

                httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelSecret", channelSecret);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", orderId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(baseUrl + apiUrl, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(result))
                {
                    var item = JsonConvert.DeserializeObject<LinePayResponse>(result);

                    if (item.returnCode == "0000")
                    {
                        var transactionId = item.info.transactionId.ToString();
                        ViewBag.paymentUrl = item.info.paymentUrl.web;
                    }
                }
                return View();
            }
        }
        public static string HashLinePayRequest(string channelSecret, string apiUrl, string body, string orderId, string key)
        {
            var request = channelSecret + apiUrl + body + orderId;

            key = key ?? "";
            // ?? 用來檢查變數的值是不是為 null
            // 如果 key 不是 null 就會傳回 key，否則傳回 ""

            var encoding = new System.Text.UTF8Encoding();

            byte[] keyByte = encoding.GetBytes(key);

            byte[] messageBytes = encoding.GetBytes(request);

            using (var hmacsha256 = new HMACSHA256(keyByte))

            {

                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

                return Convert.ToBase64String(hashmessage);

            }
        }
        [HttpGet]
        public async Task<ActionResult> confirmLinePay(string orderId, string transactionId)
        {
            using (var httpClient = new HttpClient())
            {
                var channelId = WebConfigurationManager.AppSettings["channelId"];
                var channelSecret = WebConfigurationManager.AppSettings["channelSecret"];
                var baseUrl = "https://sandbox-api-pay.line.me";
                var apiUrl = $"/v3/payments/{transactionId}/confirm";
                var requestBody = new LinePayRequest()
                {
                    amount = 400,
                    currency = "TWD"
                };
                var body = JsonConvert.SerializeObject(requestBody);

                string signature = HashLinePayRequest(channelSecret, apiUrl, body, orderId, channelSecret);

                httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelSecret", channelSecret);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", orderId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(baseUrl + apiUrl, content);
                var result = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    var item = JsonConvert.DeserializeObject<LinePayResponse>(result);

                    if (item.returnCode == "0000")
                    {
                        @ViewBag.Confirm = "付款完成";
                    }
                }
                return View();
            }
        }
    }
}