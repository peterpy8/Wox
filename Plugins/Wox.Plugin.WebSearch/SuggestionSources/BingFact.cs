using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wox.Infrastructure.Http;
using Wox.Infrastructure.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wox.Plugin.WebSearch.SuggestionSources
{
    public class BingFact : SuggestionSource
    {
        public override async Task<List<string>> Suggestions(string query)
        {
            string result;
            try
            {
                const string api = "https://www.bing.com/api/v6/search?appid=F7BE48169B408307C8C7EFCC34AA25BC51353F03&q=";
                result = await Http.Get(api + Uri.EscapeUriString(query));
            }
            catch (WebException e)
            {
                Log.Exception("|Bing.fact.Suggestions|Can't get suggestion from bing web", e);
                return new List<string>();
            }
            if (string.IsNullOrEmpty(result)) return new List<string>();

            List<string> ret = new List<string>();
            try
            {
                dynamic json = JObject.Parse(result);

                var facts = json.facts.value;
                foreach (var fact in facts)
                {
                    string desc = fact?.description;
                    if (!string.IsNullOrEmpty(desc))
                    {
                        ret.Add(desc);
                    }
                }

                var pages = json.webPages.value;
                foreach (var page in pages)
                {
                    string name = page.name;
                    string url = page.url;
                    ret.Add(name + " " + url);
                }
            }
            catch (JsonSerializationException e)
            {
                return new List<string>();
            }

            return ret;
        }

        public override string ToString()
        {
            return "Fact";
        }
    }
}
