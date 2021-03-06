﻿using System;
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
using System.Text.RegularExpressions;



namespace SearchCommon
{
    public static class PlatformSearchManager
    {
        private static double MINSCORE = double.Parse(ConfigurationManager.AppSettings["MinScore"].ToString());

        private static string host = ConfigurationManager.AppSettings["SearchBox"].ToString();

        private static string IndexName = ConfigurationManager.AppSettings["IndexName"].ToString(); //aliax name

        private static string SuggestIndexName = ConfigurationManager.AppSettings["SuggestIndexName"].ToString();

        private static int MaxSearchCount = int.Parse(ConfigurationManager.AppSettings["MaxSearchCount"].ToString());
        private static ElasticClient GetClient(string indexname)
        {
            var node = new Uri(host);

            var settings = new ConnectionSettings(
                node,
                defaultIndex: indexname
            );

            return new ElasticClient(settings);
        }

        //根据条件搜索
        public static IEnumerable<ProductInfoExt> SearchBySyntax(string keyword, out long total, out List<int> cateIds, out List<int> brandIds, out List<string> attrList, int? startprice = null, int? endprice = null, string[] brands = null, string[] catePath = null, int cateid = 0, int page = 0, int _pageSize = 50, string SortColumn = "", string SortDirection = "", int OnlyStock = 0, string FilterAttr = "")
        {
            string storeid = "";

            #region 对关键词预处理
            var suggestList = SearchSuggest(keyword);
            if (suggestList != null && suggestList.Count() > 0)
            {
                var suggest = suggestList.First();
                if (suggest != null)
                {
                    if (keyword == suggest.Text)
                    {
                        var Payload = ((dynamic)suggest.Payload);
                        if (Payload.type == "brand")
                        {
                            brands = new string[] { Payload.id + "" };
                            if (Payload.keyword != null && Payload.keyword != "")
                            {
                                keyword = Payload.keyword;
                            }
                        }
                        else if (Payload.type == "store")
                        {
                            storeid = Payload.id + "";
                            keyword = "";
                        }
                    }
                }
            }
            #endregion

            var client = GetClient(IndexName);
            var result = client.Search<ProductInfoExt>(s =>
            {
                var search = s.Index(IndexName)
                    .AllTypes()
                    .Analyzer("ik")
                    .Query(q =>
                    {
                        QueryContainer query = null;
                        
                        query &= q.QueryString(qs => qs.Query(keyword).Boost(15));

                        query &= q.Filtered(fd => fd.Filter(f =>
                        {
                            FilterContainer filter = null;
                            if (startprice != null)
                            {
                                filter &= f.Range(m => m
                                            .OnField("shopprice")
                                            .GreaterOrEquals(startprice)
                                            .Lower(endprice));
                            }
                            if (brands != null && brands.Length > 0)
                            {
                                FilterContainer q1 = null;
                                foreach (var item in brands)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                    {
                                        q1 |= f.Term("brandid", item);

                                    }
                                }
                                filter &= q1;
                            }
                            if (catePath != null && catePath.Where(a => !string.IsNullOrEmpty(a)).Count() > 0)
                            {
                                FilterContainer q1 = null;
                                foreach (var item in catePath)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                    {
                                        q1 |= f.Prefix("catePath", item);

                                    }
                                }
                                filter &= q1;
                            }
                            if (OnlyStock == 1)
                            {
                                filter &= f.Range(m => m.OnField("number").Greater(0));
                            }
                            if (!string.IsNullOrWhiteSpace(FilterAttr))
                            {
                                foreach (string item in FilterAttr.Split('|'))
                                {
                                    FilterContainer q1 = null;
                                    foreach (string attr in item.Split(','))
                                    {
                                        //q1 |= (f.Term("Attr", attr.Split('_')[0]) && f.Term("Attr", attr.Split('_')[1]));
                                        q1 |= f.Query(a => a.QueryString(aq => aq.OnFields(new List<string>() { "attr" }).Query("\"" + attr.Replace("_", ":") + "\"")));
                                    }
                                    filter &= q1;
                                }
                            }
                            if (storeid != "")
                            {
                                filter &= f.Term("storeid", storeid);
                            }
                            return filter;
                        }));
                        return query;
                    });

                //分页
                search = search.From(0).Size(MaxSearchCount);
                //排序
                if (!string.IsNullOrWhiteSpace(SortDirection) && !string.IsNullOrWhiteSpace(SortColumn))
                {
                    if (SortDirection == "ASC")
                    {
                        search = search.SortMulti(a => a.OnField(SortColumn).Ascending(), a => a.OnField("pid").Descending());
                    }
                    else
                    {
                        search = search.SortMulti(a => a.OnField(SortColumn).Descending(), a => a.OnField("pid").Descending());
                    }
                }
                return search.MinScore(MINSCORE);
            });
            total = result.Total;
            cateIds = result.Documents.Select(a => a.CateId).Distinct().ToList();
            brandIds = result.Documents.Select(a => a.BrandId).Distinct().ToList();
            attrList = result.Documents.Where(a => !string.IsNullOrWhiteSpace(a.Attr)).Select(a => a.Attr).Distinct().SelectMany(a => Regex.Split(a, "},", RegexOptions.IgnoreCase).Where(a1 => !string.IsNullOrWhiteSpace(a1)).Select(b => b + "}")).Distinct().Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
            return result.Documents.Skip((page - 1) * _pageSize).Take(_pageSize);
        }



        //http://www.chepoo.com/elasticsearch-suggest-plugin-apply.html
        /// <summary>
        /// 获取搜索建议数据
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public static IEnumerable<SuggestOption> SearchSuggest(string keyword)
        {
            var client = GetClient(SuggestIndexName);
            var r = client.Suggest<object>(s => s.Completion("my-suggest", f => f.OnField("keywords")
                                                                                    .Size(10)
                                                                                    //.Fuzzy(a=>a.Fuzziness(1))
                                                                                    .Text(keyword)));
            if (r.Suggestions.ContainsKey("my-suggest"))
            {
                var sugg = r.Suggestions["my-suggest"].First().Options;
                return sugg;
            }
            return null;
        }

        //public static void ResetIndex()
        //{
        //    var client = GetClient();

        //    var setting = new UpdateSettingsDescriptor()
        //                                                .Analysis(p => p
        //                                                    .Analyzers(m => m
        //                                                        .Add("default", new CustomAnalyzer()
        //                                                        {
        //                                                            Tokenizer = "ik"
        //                                                        })
        //                                                        ));
        //    client.DeleteIndex(IndexName);

        //    client.CreateIndex(IndexName);

        //    client.UpdateSettings(setting);
        //}


        //public static bool IndexExists()
        //{
        //    var client = GetClient();

        //    // if you end up here: check whether elasticsearch is up and running on port 9200 :-)
        //    //
        //    return client.IndexExists(IndexName).Exists;
        //}

        ///// <summary>
        ///// fulltext search by "and" Operator
        ///// </summary>
        ///// <param name="keyword"></param>
        ///// <param name="flags">1=> "and"  2 => "or"</param>
        ///// <returns></returns>
        //public static IEnumerable<ProductInfoExt> SearchFulltext(string keyword, int flags)
        //{

        //    //keyword = string.Format("*{0}*", keyword);
        //    if (string.IsNullOrEmpty(keyword))
        //    {
        //        return null;
        //    }

        //    var client = GetClient();

        //    if (flags == 1)
        //    {
        //        var result = client.Search<ProductInfoExt>(s => s
        //                                             .Index(IndexName)
        //                                             .AllTypes()
        //                                             .Query(q => q.QueryString(qs => qs.Query(keyword)))
        //                                             );
        //        return result.Documents;
        //    }
        //    else
        //    {
        //        var result = client.Search<ProductInfoExt>(s => s
        //                                            .Index(IndexName)
        //                                            .AllTypes()
        //                                            .FacetTerm(t => t.OnField(f => f.SKUGid).Size(1))
        //                                            .Query(q => q.QueryString(qs => qs.Query(keyword)))
        //                                            );
        //        return result.Documents;
        //    }
        //}


    }
}
