using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Task5.Helpers;

namespace Task5.Controllers
{
    public class AmazonS3Controller : ApiController
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 s3Client;
        private static readonly string bucketName = "matt-task5-images";

        [HttpPost]
        [Route("api/amazons3")]
        public IHttpActionResult UploadToS3()
        {
            var file = HttpContext.Current.Request.Files[0];

            s3Client = new AmazonS3Client(
                new AmazonS3Config
                {
                    // Retries and Timeouts
                    Timeout = TimeSpan.FromSeconds(10),
                    ReadWriteTimeout = TimeSpan.FromSeconds(20),
                    MaxErrorRetry = 5,

                    RegionEndpoint = bucketRegion
                });
            
            Guid guid = Guid.NewGuid();
            string key = "images/" + guid + "-" + file.FileName;

            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = key
                };

                using (Stream inputStream = file.InputStream)
                {
                    request.InputStream = inputStream;

                    PutObjectResponse response = s3Client.PutObject(request);
                }

                // Construct URL to the file
                var fileURL = "https://" + bucketName + ".s3.amazonaws.com/" + key;

                var shortURL = BitlyLink.shortenLink(fileURL).Result;

                return Ok(shortURL);

            }
            catch (AmazonS3Exception e)
            {
                return BadRequest("Error encountered on server. Message:'{0}' when writing an object: " + e.Message);
            
            }
            catch (Exception e)
            {
                return BadRequest("Unknown encountered on server. Message:'{0}' when writing an object: " + e.Message);
            }

        }
    }
}
