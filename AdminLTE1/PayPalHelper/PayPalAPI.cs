﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdminLTE1.PayPalHelper
{
    public class PayPalAPI
    {
        public IConfiguration configuration { get; }
        public PayPalAPI(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task<string> getRedirectURLToPayPal(double total, string currency, string orderId)
        {
            try
            {
                return Task.Run(async () =>
                {
                    HttpClient http = GetPaypalHttpClient();
                    PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(http);


                    PayPalPaymentCreatedResponse createdPayment = await CreatePaypalPaymentAsync(http, accessToken, total, currency, orderId);
                    return createdPayment.links.First(x => x.rel == "approval_url").href;
                }).Result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Failed to login to payPal");
                return null;
            }
        }

        public async Task<PayPalPaymentExecutedResponse> executedPayment(string paymentId, string payerId)
        {
            try
            {
                HttpClient http = GetPaypalHttpClient();
                PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(http);
                return await ExecutePaypalPaymentAsync(http, accessToken, paymentId, payerId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Failed to login to PayPal");
                return null;
            }
        }

        private HttpClient GetPaypalHttpClient()
        {
            string sandbox = configuration["PayPal:urlAPI"];

            var http = new HttpClient
            {
                BaseAddress = new Uri(sandbox),
                Timeout = TimeSpan.FromSeconds(30),
            };

            return http;
        }

        private async Task<PayPalAccessToken> GetPayPalAccessTokenAsync(HttpClient http)
        {
            try
            {


                byte[] bytes = Encoding.GetEncoding("iso-8859-1")
                    .GetBytes($"{configuration["PayPal:clientId"]}:{configuration["PayPal:secret"]}");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

                var form = new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials"
                };

                request.Content = new FormUrlEncodedContent(form);

                HttpResponseMessage response = await http.SendAsync(request);

                string content = await response.Content.ReadAsStringAsync();
                PayPalAccessToken accessToken = JsonConvert.DeserializeObject<PayPalAccessToken>(content);
                ///content; //JsonConvert.DeserializeObject<PayPalAccessToken>(File.ReadAllText(content));
                return accessToken;
            }
            catch (Exception exe)
            {

                throw;
            }
        }


        private async Task<PayPalPaymentCreatedResponse> CreatePaypalPaymentAsync(HttpClient http,
            PayPalAccessToken accessToken, double total, string currency, string orderId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "v1/payments/payment");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var payment = JObject.FromObject(new
            {
                intent = "sale",
                redirect_urls = new
                {
                    return_url = configuration["PayPal:returnUrl"],
                    cancel_url = configuration["PayPal:cancelUrl"]
                },
                payer = new { payment_method = "paypal" },



                transactions = JArray.FromObject(new[]
                {
                    new
                    {
                        amount = new
                        {
                            total = total,
                            currency = currency
                        },
                        description="The payment transaction description.",
                       custom = orderId,
                       soft_descriptor= "ECHI5786786"
                    }
                })
            });

            request.Content = new StringContent(JsonConvert.SerializeObject(payment),
Encoding.UTF8, "application/json");

            HttpResponseMessage response = await http.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            PayPalPaymentCreatedResponse paypalPaymentCreated =
                JsonConvert.DeserializeObject<PayPalPaymentCreatedResponse>(content);
            return paypalPaymentCreated;
        }

        private async Task<PayPalPaymentExecutedResponse> ExecutePaypalPaymentAsync(HttpClient http,
            PayPalAccessToken accessToken, string paymentId, string payerId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"v1/payments/payment/{paymentId}/execute");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var payment = JObject.FromObject(new
            {
                payer_id = payerId
            });

            request.Content = new StringContent(JsonConvert.SerializeObject(payment),
                Encoding.UTF8, "application/json");

            HttpResponseMessage response = await http.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            PayPalPaymentExecutedResponse executedPayment =
                JsonConvert.DeserializeObject<PayPalPaymentExecutedResponse>(content);
            return executedPayment;
        }
    }
}
