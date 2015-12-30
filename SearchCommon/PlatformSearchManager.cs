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
        private static string IndexName = ConfigurationManager.AppSettings["IndexName"].ToString(); //aliax name

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



            var setting = new UpdateSettingsDescriptor()
                                                        .Analysis(p => p
                                                            .Analyzers(m => m
                                                                .Add("default", new CustomAnalyzer()
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
                                                    .FacetTerm(t => t.OnField(f => f.SKUGid).Size(1))
                                                    .Query(q => q.QueryString(qs => qs.Query(keyword)))
                                                    );
                return result.Documents;
            }
        }


        //根据条件搜索
        public static IEnumerable<ProductInfoExt> SearchBySyntax(string keyword, out long total, int? startprice = null, int? endprice = null, string[] brands = null, string catePath = null, int cateid = 0, int page = 0, int _pageSize = 50, string SortColumn = "", string SortDirection = "", int OnlyStock = 0, string FilterAttr="")
        {
            var client = GetClient();

            // int[] numbers = { 4, 5, 6, 1, 2, 3, -2, -1, 0 };


            total = client.Count<ProductInfoExt>(s => 
            {
                var search = s.Index(IndexName)
                    .AllTypes()
                    .Analyzer("ik")
                    .Query(q =>
                    {
                        QueryContainer query = null;
                        if (startprice != null)
                        {
                            query &= q.Range(m => m
                                        .OnField(f => f.Shopprice)
                                        .GreaterOrEquals(startprice)
                                        .Lower(endprice));
                        }


                        if (brands != null && brands.Length > 0)
                        {
                            foreach (var item in brands)
                            {
                                query = query || q.Term("brandid", item);
                            }
                        }


                        if (catePath != null)
                        {
                            query &= q.Prefix("catePath", catePath);
                            
                        }


                        //if (cateid != 0)
                        //{
                        //    query &= q.Term("cateid", cateid);
                        //}

                        if (OnlyStock == 1)
                        {
                            query &= q.Range(m => m.OnField(f => f.Number).Greater(0));
                        }

                        if (!string.IsNullOrWhiteSpace(FilterAttr))
                        {
                            foreach (string item in FilterAttr.Split('|'))
                            {
                                QueryContainer q1 = null;
                                foreach (string attr in item.Split(','))
                                {
                                    q1 |= (q.Term("Attr", attr.Split('_')[0]) & q.Term("Attr", attr.Split('_')[1]));
                                }
                                query &= q1;
                            }
                        }

                        query &= q.QueryString(qs => qs.Query(keyword));

                        return query;
                    });
                return search;
            }).Count;





            var result = client.Search<ProductInfoExt>(s =>
            {
                var search = s.Index(IndexName)
                    .AllTypes()
                    .Analyzer("ik")
                    .Query(q =>
                    {
                        QueryContainer query = null;
                        if (startprice != null)
                        {
                            query &= q.Range(m => m
                                        .OnField(f => f.Shopprice)
                                        .GreaterOrEquals(startprice)
                                        .Lower(endprice));
                        }


                        if (brands != null && brands.Length > 0)
                        {
                            foreach (var item in brands)
                            {
                                query = query || q.Term("brandid", item);
                            }
                        }


                        if (catePath != null)
                        {
                            query &= q.Prefix("catePath", catePath);
                        }


                        //if (cateid != 0)
                        //{
                        //    query &= q.Term("cateid", cateid);
                        //}

                        if (OnlyStock == 1)
                        {
                            query &= q.Range(m => m.OnField(f => f.Number).Greater(0));
                        }
                        if (!string.IsNullOrWhiteSpace(FilterAttr))
                        {
                            foreach (string item in FilterAttr.Split('|'))
                            {
                                QueryContainer q1 = null;
                                foreach (string attr in item.Split(','))
                                {
                                    q1 |= (q.Term("Attr", attr.Split('_')[0]) & q.Term("Attr", attr.Split('_')[1]));
                                }
                                query &= q1;
                            }
                        }
                        query &= q.QueryString(qs => qs.Query(keyword));

                        return query;
                    });

                //分页
                search = search.From((page-1) * _pageSize).Size(_pageSize);
                //排序
                if (!string.IsNullOrWhiteSpace(SortDirection) && !string.IsNullOrWhiteSpace(SortColumn))
                {
                    if (SortDirection == "ASC")
                    {
                        search = search.SortAscending(SortColumn);
                    }
                    else
                    {
                        search = search.SortDescending(SortColumn);
                    }
                }
                return search;
            });
            return result.Documents;

            //var list = new List<ProductInfoExt>();

            //foreach (var item in result.Documents.GroupBy(s=>s.SKUGid))
            //{
            //    list.Add(item.First());
            //}


            //total = list.Count;

            //var rs = list.Take(_pageSize).Skip(_pageSize * page).ToList();

            //return rs;
        }


        public static IEnumerable<ProductInfoExt> SearchProudct(string keyword, int page = 0, int _pageSize = 20)
        {
            var client = GetClient();

            var result = client.Search<ProductInfoExt>(s => s
                .Index(IndexName)
                .AllTypes()
                .Analyzer("ik")
                .From(page * _pageSize)
                .Size(_pageSize)
                .Query(q => q.QueryString(qs => qs.Query(keyword))));


            return result.Documents;
        }


        //获取分类
        public static IEnumerable<int> SearchCateIds(string keyword, string[] brands, int? startprice = null, int? endprice = null, string catePath = null, int OnlyStock=0)
        {
            var client = GetClient();

            var result = client.Search<ProductInfoExt>(s => s
                         .Index(IndexName)
                         .AllTypes()
                         .Analyzer("ik")
                         .Source(f => f.Include("cateid"))
                         .Size(int.MaxValue)
                         .Query(q =>
                         {
                             QueryContainer query = null;
                             if (startprice != null)
                             {
                                 query &= q.Range(m => m
                                             .OnField(f => f.Shopprice)
                                             .GreaterOrEquals(startprice)
                                             .Lower(endprice));
                             }


                             if (brands != null && brands.Length > 0)
                             {
                                 foreach (var item in brands)
                                 {
                                     query = query || q.Term("brandid", item);
                                 }
                             }


                             if (catePath != null)
                             {
                                 query &= q.Prefix("catePath", catePath);
                             }

                             if (OnlyStock == 1)
                             {
                                 query &= q.Range(m => m.OnField(f => f.Number).Greater(0));
                             }

                             query &= q.QueryString(qs => qs.Query(keyword));

                             return query;
                         }));
            return result.Documents.Select(p => p.CateId).Distinct();
        }


        //获取品牌
        public static IEnumerable<int> SearchBrands(string keyword, int? startprice = null, int? endprice = null, string catePath = null, int OnlyStock=0)
        {
            var client = GetClient();

            var result = client.Search<ProductInfoExt>(s => s
                         .Index(IndexName)
                         .AllTypes()
                         .Analyzer("ik")
                         .Source(f => f.Include("brandid"))
                         .Size(int.MaxValue)
                         .Query(q =>
                         {
                             QueryContainer query = null;
                             if (startprice != null)
                             {
                                 query &= q.Range(m => m
                                           .OnField(f => f.Shopprice)
                                           .GreaterOrEquals(startprice)
                                           .Lower(endprice));
                             }

                             if (catePath!=null)
                             {
                                 query &= q.Prefix("catePath", catePath);

                             }

                             if (OnlyStock == 1)
                             {
                                 query &= q.Range(m => m.OnField(f => f.Number).Greater(0));
                             }

                             query &= q.QueryString(qs => qs.Query(keyword));
                             return query;
                         }));

            return result.Documents.Select(p => p.BrandId).Distinct();
        }



        //http://www.chepoo.com/elasticsearch-suggest-plugin-apply.html

        public static List<string> SearchSuggest(string keyword)
        {

            var client = GetClient();


            // depending on your map --> index --> query configuraion there are different methods to get suggestions:
            //
            // a.) my favourite
            //
            //SearchDescriptor<BlogArticle> descriptor = new SearchDescriptor<BlogArticle>();
            //descriptor = descriptor.SuggestCompletion("suggest", c => c.OnField("title").Text(SearchTerm));

            //var ... = GetClient().Search<BlogArticle>(s => descriptor);

            // b.) common
            //
            var r = GetClient().Suggest<ProductInfoExt>(s => s.Phrase("my-suggest", f => f.OnField("name")
                                                                                    .GramSize(1)
                                                                                    .Size(5)
                                                                                    .MaxErrors((decimal)0.5)
                                                                                    .DirectGenerator(g => g.MinWordLength(3)
                                                                                                            .OnField("name")
                                                                                                            .SuggestMode(SuggestMode.Always))
                                                                                    .Text(keyword)));


            var rs = GetClient().Suggest<ProductInfoExt>(m => m.Term("suggest-test", q => q.OnField("name").Size(10).Text(keyword)));


           


            var list = new List<string>();


            var gg = rs.Suggestions["suggest-test"].First().Options;


            var sugg = r.Suggestions["my-suggest"].First().Options;


            if (sugg.Count() > 0)
            {
                foreach (SuggestOption opt in sugg)
                {
                    list.Add(opt.Text);
                }
            }

            return list;


            //List<String> suggestions = new SuggestRequestBuilder()
            //    .field("content")//查询field
            //    .term(q)//提示关键字
            //    .size(n)//返回结果数
            //    .similarity(0.5f)//相似度
            //    .execute().actionGet().suggestions();

            //return null;
        }
    }
}
