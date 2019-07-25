﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wox.Infrastructure.Http;
using Wox.Infrastructure.Logger;
using HtmlAgilityPack;

namespace Wox.Plugin.WebSearch.SuggestionSources
{
    public class BingDict : SuggestionSource
    {
        public override async Task<List<string>> Suggestions(string query)
        {
            string result;
            try
            {
                const string api = "https://www.bing.com/AS/Suggestions?scope=dictionary&pt=page.bingdict&bq=welcome&mkt=zh-cn&ds=bingdict&cp=1&cvid=BE800721CDE94A90BDE643143D889742&qry=";
                result = await Http.Get(api + Uri.EscapeUriString(query));
            }
            catch (WebException e)
            {
                Log.Exception("|Bing.web.Suggestions|Can't get suggestion from bing web", e);
                return new List<string>();
            }
            if (string.IsNullOrEmpty(result)) return new List<string>();

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(result);

            var htmlNodes = htmlDoc.DocumentNode.Descendants("li");
            return htmlNodes.Select(node => node.Attributes["query"].Value).ToList();
        }

        public override string ToString()
        {
            return "Bing Dictionary";
        }
    }
}