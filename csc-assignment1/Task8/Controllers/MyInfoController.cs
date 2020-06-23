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
        [HttpGet]
        [Route("callback")]
        public RedirectResult Callback([FromUri] string code)//[FromUri] string code, [FromUri] string state
        {
                return Redirect("http://localhost:3001/Home/Index");        
        }

    }
}
