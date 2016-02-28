using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;


namespace LinkenLabs.Market.RestApi.Core
{
    public static class Google
    {
        public static string GetShortUrl(string longUrl, string apiKey)
        {
            string apiUrl = "https://www.googleapis.com/urlshortener/v1/url?key=";
            WebClient client = new WebClient();
            client.Headers["Content-Type"] = "application/json";
            var response = client.UploadString(apiUrl + apiKey, JsonConvert.SerializeObject(new { longUrl = longUrl }));
            var shortUrl = (string)JObject.Parse(response)["id"];
            return shortUrl;
        }

        public static void Test()
        {
            string apiKey = @"AIzaSyCGgiWzPrCZapovxBnVCzSh0jTvuun0c4M";
            string longUrl = @"https://www.mooketplace.com/zh-TW/market/062562b1-9cbf-4a1c-b27e-75e5627439e6";

            string shortUrl = GetShortUrl(longUrl, apiKey);
        }
    }
}