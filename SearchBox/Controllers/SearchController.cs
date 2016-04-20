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
                string[] brandids = null;

                if (brands != null)
                {
                    brandids = brands.Split(',');
                }

                string[] catepaths = null;
                if (catePath != null)
                {
                    catepaths = catePath.Split('|');
                }

                

                //return  PlatformSearchManager.SearchFulltext("电视", 0);
                //return  PlatformSearchManager.SearchProudct("头层牛皮凉鞋", 1);
                // return   PlatformSearchManager.SearchBySyntax("头层牛皮凉鞋",220,230);

                // return PlatformSearchManager.SearchCateIds("头层牛皮凉鞋");

                //return PlatformSearchManager.SearchBrands("头层牛皮凉鞋");
                //SearchBySyntax(string keyword, out long total, int? startprice = null, int? endprice = null, string[] brands = null, string catePath = null, int cateid = 0, int page = 0, int _pageSize = 50, string SortColumn = "", string SortDirection = "", int OnlyStock = 0, string FilterAttr="")
                long total = 0;
                List<string> attrlist = new List<string>();
                List<int> cateIds = new List<int>();
                List<int> brandIds = new List<int>();
                var productList = PlatformSearchManager.SearchBySyntax(keyword, out total,out cateIds,out brandIds,out attrlist, startprice, endprice, brandids, catepaths, cateid, page, pageSize, SortColumn, SortDirection, OnlyStock, FilterAttr);
                var rs = new
                {
                    brandids = brandIds,
                    cateids = cateIds,
                    list = productList,
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


        [HttpGet]
        public List<string> SearchSuggest(string keyword)
        {
            List<string> list = new List<string>();
            var sugg = PlatformSearchManager.SearchSuggest(keyword);
            if (sugg != null && sugg.Count() > 0)
            {
                foreach (var opt in sugg)
                {
                    list.Add(opt.Text);
                }
            }
            return list;
        }

    }
}
