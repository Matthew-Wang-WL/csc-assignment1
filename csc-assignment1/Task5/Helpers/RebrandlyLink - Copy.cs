using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class Rebrandly
    {

        public static async Task<string> shortenLink(string fileUrl, string fileName)
        {
            //Get current date and time in ISO 8601 format
            string timestamp = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            string dateOnly = timestamp.Split('T')[0];
            string shortName = fileName.Substring(0, 7);

            string slashTag = "ST0280-" + dateOnly + "-" + shortName;
            //Debug.WriteLine("slashTag" + slashTag);

            var payload = new
            {
                destination = fileUrl,
                domain = new
                {
                    fullName = "rebrand.ly"
                },
                slashtag = slashTag,

            };

            using (var httpClient = new HttpClient { BaseAddress = new Uri("https://api.rebrandly.com") })
            {
                httpClient.DefaultRequestHeaders.Add("apikey", "9cd97b0f2d0849ab834b2c24f31e72ad");
                httpClient.DefaultRequestHeaders.Add("workspace", "bab0d37d53834163a4260407cb2d740a");

                var body = new StringContent(
                    JsonConvert.SerializeObject(payload), UnicodeEncoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("/v1/links", body).ConfigureAwait(false))
                {
                    Debug.WriteLine(response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    response.EnsureSuccessStatusCode();

                    var link = JsonConvert.DeserializeObject<dynamic>(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                    Debug.WriteLine($"Long URL was {payload.destination}, short URL is {link.shortUrl}");

                    return link.shortUrl;
                }
            }
        }
    }
}