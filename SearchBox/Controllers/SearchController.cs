using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using SearchCommon;
using SearchBox.Models;

namespace SearchBox.Controllers
{
    public class SearchController : ApiController
    {
        [HttpGet]
        public IEnumerable<ProductInfoExt> SearchProducts()
        {
          //return  PlatformSearchManager.SearchFulltext("电视", 0);
          //return  PlatformSearchManager.SearchProudct("头层牛皮凉鞋", 1);
          return   PlatformSearchManager.SearchBySyntax("头层牛皮凉鞋",220,230);
        }
    }
}
