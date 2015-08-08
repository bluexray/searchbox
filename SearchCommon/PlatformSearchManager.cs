using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using Nest;
using Nest.DSL;
using Nest.Domain;
using Elasticsearch.Net;
using SearchBox.Models;



namespace SearchCommon
{
    public static class PlatformSearchManager
    {

        private static string host = ConfigurationManager.AppSettings["SearchBox"].ToString();

        /// <summary>
        /// Like a database name (fake indexname)
        /// </summary>
        private static string IndexName = "testmall";

        /// <summary>
        /// Defaults for paging
        /// </summary>
        private static int pageSize = 20;

        /// <summary>
        /// How many results should a query return max?
        /// </summary>
        private static int maxResults = 400;



        public static string PreHighlightTag
        {
            get { return @"<strong>"; }
        }

        public static string PostHighlightTag
        {
            get { return @"</strong>"; }
        }




        // Basis für Http-Verbindung (REST)
        //
        private static ElasticClient GetClient()
        {
            var node = new Uri(host);

            var settings = new ConnectionSettings(
                node,
                defaultIndex: IndexName
            );

            return new ElasticClient(settings);
        }


        public static void ResetIndex()
        {
            var client = GetClient();



            var  setting = new UpdateSettingsDescriptor()
                                                        .Analysis(p=>p
                                                            .Analyzers(m=>m
                                                                .Add("default",new CustomAnalyzer()
                                                                {
                                                                    Tokenizer = "ik"
                                                                })
                                                                ));
            client.DeleteIndex(IndexName);

            client.CreateIndex(IndexName);

            client.UpdateSettings(setting);
        }


        public static bool IndexExists()
        {
            var client = GetClient();

            // if you end up here: check whether elasticsearch is up and running on port 9200 :-)
            //
            return client.IndexExists(IndexName).Exists;
        }

        /// <summary>
        /// fulltext search by "and" Operator
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="flags">1=> "and"  2 => "or"</param>
        /// <returns></returns>
        public static IEnumerable<ProductInfoExt> SearchFulltext(string keyword, int flags)
        {

            //keyword = string.Format("*{0}*", keyword);
            if (string.IsNullOrEmpty(keyword))
            {
                return null;
            }

            var client = GetClient();
            
            if (flags == 1)
            {
                var result = client.Search<ProductInfoExt>(s => s
                                                     .Index(IndexName)
                                                     .AllTypes()
                                                     .Query(q => q.QueryString(qs => qs.Query(keyword)))
                                                     );
                return result.Documents;
            }
            else
            {
                var result = client.Search<ProductInfoExt>(s => s
                                                    .Index(IndexName)
                                                    .AllTypes()
                                                    .Query(q => q.QueryString(qs => qs.Query(keyword)))
                                                    );
                return result.Documents;
            }
        }


        //根据条件搜索
        public static IEnumerable<ProductInfoExt> SearchBySyntax(string keyword, int? startprice=null, int? endprice=null, int brandid=0, int cateid=0, int page = 0, int _pageSize = 50)
        {
            var client = GetClient();

            var result = client.Search<ProductInfoExt>(s => s
                                .Index(IndexName)
                                .AllTypes()
                                .Analyzer("ik")
                                .From(page * _pageSize)
                                .Size(_pageSize)
                                .Query(q => { 
                                        QueryContainer query = null;
                                    if (startprice!=null)
                                    {
                                        query &= q.Range(m => m
                                                  .OnField(f => f.Shopprice)
                                                  .GreaterOrEquals(startprice)
                                                  .Lower(endprice));
                                    }
                                    if (brandid!=0)
                                    {

                                        query &= q.Term("brandid", brandid);
                                    }

                                    if (cateid!=0)
                                    {
                                        query &= q.Term("cateid", cateid);
                                    }

                                    query &= q.QueryString(qs => qs.Query(keyword));
                                    return query;        
                                            }));
            return result.Documents;
        }


        public static IEnumerable<ProductInfoExt> SearchProudct(string keyword, int page=0, int _pageSize=20)
        {
            var client = GetClient();

            var result = client.Search<ProductInfoExt>(s => s
                .Index(IndexName)
                .AllTypes()
                .Analyzer("ik")     
                .From(page* _pageSize)
                .Size(_pageSize)
                .Query(q => q.QueryString(qs => qs.Query(keyword))));


            return result.Documents;
        }


        //获取分类
        public static IEnumerable<int> SearchCateIds(string keyword,int brandid=0, int? startprice = null, int? endprice = null)
        {
            var client = GetClient();

            var result = client.Search<ProductInfoExt>(s => s
                         .Index(IndexName)
                         .AllTypes()
                         .Analyzer("ik")
                         .Source(f => f.Include("cateid"))
                         .Query(q => {
                             QueryContainer query = null;
                             if (startprice != null)
                             {
                                 query &= q.Range(m => m
                                           .OnField(f => f.Shopprice)
                                           .GreaterOrEquals(startprice)
                                           .Lower(endprice));
                             }

                             if (brandid != 0)
                             {

                                 query &= q.Term("brandid", brandid);
                             }

                             query &= q.QueryString(qs => qs.Query(keyword));
                             return query;
                             }));

            return result.Documents.Select(p=>p.CateId);
        }


        //获取品牌
        public static IEnumerable<int> SearchBrands(string keyword, int cateid = 0, int? startprice = null, int? endprice = null)
        {
            var client = GetClient();

            var result = client.Search<ProductInfoExt>(s => s
                         .Index(IndexName)
                         .AllTypes()
                         .Analyzer("ik")
                         .Source(f => f.Include("brandid"))
                         .Query(q => {
                             QueryContainer query = null;
                             if (startprice != null)
                             {
                                 query &= q.Range(m => m
                                           .OnField(f => f.Shopprice)
                                           .GreaterOrEquals(startprice)
                                           .Lower(endprice));
                             }

                             if (cateid != 0)
                             {

                                 query &= q.Term("cateid", cateid);
                             }

                             query &= q.QueryString(qs => qs.Query(keyword));
                             return query;
                         }));

            return result.Documents.Select(p=>p.BrandId);
        }
    }
}
