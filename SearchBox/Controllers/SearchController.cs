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
        [HttpPost]
        public object SearchProducts(string keyword, string brands = null, int? startprice = null, int? endprice = null, string catePath = null, int cateid = 0, int page = 0, int pageSize = 50, string SortColumn = "", string SortDirection = "", int OnlyStock = 0, string FilterAttr = "")
        {
            try
            {
                Utils.WriteLogFile(Request.RequestUri.ToString());
                string[] arry = null;

                if (brands != null)
                {
                    arry = brands.Split(',');
                }

                string[] catepaths = null;
                if (catePath != null)
                {
                    catepaths = catePath.Split('|');
                }

                long total = 0;
                List<string> attrlist = new List<string>();

                //return  PlatformSearchManager.SearchFulltext("电视", 0);
                //return  PlatformSearchManager.SearchProudct("头层牛皮凉鞋", 1);
                // return   PlatformSearchManager.SearchBySyntax("头层牛皮凉鞋",220,230);

                // return PlatformSearchManager.SearchCateIds("头层牛皮凉鞋");

                //return PlatformSearchManager.SearchBrands("头层牛皮凉鞋");
                //SearchBySyntax(string keyword, out long total, int? startprice = null, int? endprice = null, string[] brands = null, string catePath = null, int cateid = 0, int page = 0, int _pageSize = 50, string SortColumn = "", string SortDirection = "", int OnlyStock = 0, string FilterAttr="")
                var rs = new
                {
                    brandids = PlatformSearchManager.SearchBrands(keyword, startprice, endprice, catepaths),
                    cateids = PlatformSearchManager.SearchCateIds(keyword, arry, startprice, endprice, catepaths),
                    list = PlatformSearchManager.SearchBySyntax(keyword, out total, out attrlist, startprice, endprice, arry, catepaths, cateid, page, pageSize, SortColumn, SortDirection, OnlyStock, FilterAttr),
                    total = total,
                    attrlist = attrlist
                };
                return rs;
            }
            catch (Exception ex)
            {
                Utils.WriteLogFile(ex.ToString());
                return ex.Message;
            }
        }


        //[HttpPost]
        //public IEnumerable<int> SearchCateids(string keyword, string brands = null, int? startprice = null, int? endprice = null, string catePath = null)
        //{
        //    string[] arry = null;

        //    if (brands != null)
        //    {
        //        arry = brands.Split(',');
        //    }

        //    return PlatformSearchManager.SearchCateIds(keyword, arry, startprice, endprice, catePath);
        //}

        //[HttpPost]
        //public IEnumerable<int> SearchBrands(string keyword, int cateid, int? startprice = null, int? endprice = null, string catePath = null)
        //{
        //    return PlatformSearchManager.SearchBrands(keyword,startprice, endprice, catePath);
        //}

        //[HttpGet]
        //public IEnumerable<ProductInfoExt> Search(string keyword)
        //{
        //    long total = 0;
        //    return PlatformSearchManager.SearchBySyntax(keyword, out total, null, null, null, "1", 0, 0, 50);
        //}

        //[HttpGet]
        //public IEnumerable<ProductInfoExt> SearchTest(string keyword)
        //{

        //    return PlatformSearchManager.SearchFulltext(keyword, 0);

        //}


        //[HttpGet]
        //public void SearchSuggest(string keyword)
        //{
        //    var s = PlatformSearchManager.SearchSuggest(keyword);
        //}

    }
}
