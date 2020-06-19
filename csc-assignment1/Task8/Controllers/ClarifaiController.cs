using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Clarifai.DTOs.Predictions;
using System.IO;
using System.Web;
using System.Web.Http.Results;

namespace Task8.Controllers
{
    public class ClarifaiController : ApiController
    {
        [HttpPost]
        [Route("api/clarifai")]
        public async Task<IHttpActionResult> PredictImage()
        {
            //Retrieve file
            var file = HttpContext.Current.Request.Files[0];
            //Convert hhtpfile into byte array
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }

            try
            {
                // When passed in as a string
                var client = new ClarifaiClient("2f4177e6df0a4055b346e9e68255db48");

                // When using async/await, via bytes
                var res = await client.Predict<Concept>(
                       "Receipt_Detection",
                       new ClarifaiFileImage(fileData),
                       modelVersionID: "c659dcd1b7af4e31b0c5679a745fc784")
                .ExecuteAsync();
             
                if (res.IsSuccessful)
                {
                    var concepts = res.Get().Data.Select(c => $"{c.Name}:{c.Value}");
                    var body = string.Join(", ", concepts);

                    return Ok(body);
                }
                else
                {
                    return BadRequest($"The request was not successful: {res.Status.Description}");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Something went wrong: {e.Message}");
            }
        }
    }
}
 