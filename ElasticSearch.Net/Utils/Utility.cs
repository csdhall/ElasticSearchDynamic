using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ElasticSearch.Net.Utils
{
    public class Utility
    {
        public static bool StartsWithProtocol(string regexEx)
        {
            string value = Regex.Match(regexEx, @"^(?<protocol>.+://)?").Groups["protocol"].Captures[0].Value;
            if (!String.IsNullOrEmpty(value))
                return true;
            return false;
        }

        public static dynamic UrlParse(string config, bool parseQueryString, bool parseProtocolAndHost,
            string[] urlParsedFields)
        {
            //TODO: Check for these values dynamically
            //{ "protocol", "hostname", "pathname", "port", "auth", "query" };
            var uri = new Uri(config);
            dynamic parsedUri = new ExpandoObject();
           
            parsedUri.protocol = uri.Scheme;
            parsedUri.hostname = uri.Host;
            parsedUri.pathname = uri.LocalPath;
            parsedUri.port = uri.Port;
            //TODO: Add later 
            //  parsedUri.auth = uri.auth;
            parsedUri.query = uri.Query;

            if (parseQueryString)
            {
                //TODO: Add later
            }

            return parsedUri;
        }

        public static string ParseQueryString(string query)
        {
            var dict = HttpUtility.ParseQueryString(query);
            return new JavaScriptSerializer().Serialize(
                                dict.AllKeys.ToDictionary(k => k, k => dict[k])
                       );
        }

      
    }
}
