using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SearchBox.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage RequestToken(string ConsumerKey, string ConsumerSecret)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage AccessToken(string ConsumerKey, string Verifier)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
