using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Task8.Controllers
{
    public class MyInfoController : ApiController
    {
        private static readonly HttpClient client = new HttpClient();


        [HttpPost]
        //[Route("token")]
        public async Task<IHttpActionResult> Token([FromUri] string code)
        {
            var _clientSecret = "44d953c796cccebcec9bdc826852857ab412fbe2";
            var _redirectUrl = "http://localhost:3001/callback";
            var _clientId = "STG2-MYINFO-SELF-TEST";

            var values = "?grant_type=authorization_code" + "&code=" + code + "&redirect_uri=" + _redirectUrl + "&client_id=" + _clientId + "&client_secret=" + _clientSecret;
            var stringContent = new StringContent(values, Encoding.UTF8, "application/x-www-form-urlencoded");
            Debug.WriteLine(stringContent);
            try
            {
                var response = await client.PostAsync("https://sandbox.api.myinfo.gov.sg/com/v3/token", stringContent);

                var responseString = await response.Content.ReadAsStringAsync();
                //var tokenUrl = "https://sandbox.api.myinfo.gov.sg/com/v3/token?grant_type=authorization_code" + "&code=" + code +"&redirect_uri=" + _redirectUrl +"&client_id=" + _clientId +"&client_secret=" + _clientSecret;
                Console.WriteLine(responseString);

                return Ok(responseString);
            }
            catch (Exception e)
            {
                return BadRequest($"Something went wrong: {e.Message}");
            }
        }

        [HttpGet]
        [Route("callback")]
        public RedirectResult Callback([FromUri] string code)//[FromUri] string code, [FromUri] string state
        {
                return Redirect("http://localhost:3001/Home/Index");        
        }

    }
}
