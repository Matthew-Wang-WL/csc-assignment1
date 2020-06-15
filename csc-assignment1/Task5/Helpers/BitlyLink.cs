using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Task5.Helpers
{
    public class BitlyLink
    {

        public static async Task<string> shortenLink(string URL)
        {
            string bitlyAccessToken = "67c347156c512b2563b3af45477816e63f63e4a2";
            string myJson = JsonConvert.SerializeObject(new {
                    long_url = URL,
                });

            // API link
            string destinationURL = "https://api-ssl.bitly.com/v4/shorten";

            HttpClientHandler hch = new HttpClientHandler();
            hch.Proxy = null;
            hch.UseProxy = false;
            HttpClient client = new HttpClient(hch);

            // Set authorization header
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bitlyAccessToken);

            System.Diagnostics.Debug.WriteLine("destURL: " + destinationURL);
            System.Diagnostics.Debug.WriteLine("myJson: " + myJson);

            HttpResponseMessage response = await client.PostAsync(
                destinationURL,
                new StringContent(myJson, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            if(response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("Link Created");

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var data = (JObject)JsonConvert.DeserializeObject(jsonResponse);
                string link = data["link"].Value<string>();
                return link;
            } else
            {
                System.Diagnostics.Debug.WriteLine("Failed to create link");
                return "Failed to create link";
            }


        }
    }
}