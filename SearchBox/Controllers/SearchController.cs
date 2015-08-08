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
        public IEnumerable<ProductInfoExt> SearchProducts(string keyword, int? startprice = null, int? endprice = null, int brandid = 0, int cateid = 0, int page = 0, int pageSize = 50)
        {
            //return  PlatformSearchManager.SearchFulltext("电视", 0);
            //return  PlatformSearchManager.SearchProudct("头层牛皮凉鞋", 1);
            // return   PlatformSearchManager.SearchBySyntax("头层牛皮凉鞋",220,230);

            // return PlatformSearchManager.SearchCateIds("头层牛皮凉鞋");

            //return PlatformSearchManager.SearchBrands("头层牛皮凉鞋");
            return PlatformSearchManager.SearchBySyntax(keyword, startprice, endprice, brandid, cateid, page, pageSize);
        }

        [HttpPost]
        public IEnumerable<int> SearchCateids(string keyword,int brandid=0,int? startprice= null,int? endprice = null)
        {
           return PlatformSearchManager.SearchCateIds(keyword, brandid, startprice, endprice);
        }


        public IEnumerable<int> SearchBrands(string keyword, int cateid = 0, int? startprice = null, int? endprice = null)
        {
            return PlatformSearchManager.SearchBrands(keyword, cateid, startprice, endprice);
        }
    }
}
