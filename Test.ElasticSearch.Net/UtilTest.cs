using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ElasticSearch.Net.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.ElasticSearch.Net
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void Regex_WhenStartsWithProtocol_ReturnTrue()
        {
            string s = Regex.Match(@"http://www.foo.com/bar/123/abc", @"^(?<protocol>.+://)?(?<site>[^/]+)/(?:(?<part>[^/]+)/?)*$").Groups["part"].Captures[0].Value;
            Group protocol = Regex.Match(@"http://www.foo.com/bar/123/abc", @"^(?<protocol>.+://)?(?<site>[^/]+)/(?:(?<part>[^/]+)/?)*$").Groups["protocol"];
            string value = Regex.Match(@"http://www.foo.com/bar/123/abc", @"^(?<protocol>.+://)?").Groups["protocol"].Captures[0].Value;

            Assert.AreEqual(@"http://", value);
        }

        [TestMethod]
        public void Regex_WhenPassConfigValues_ReturnParsedUrl()
        {
            string[] urlParsedFields = { "protocol", "hostname", "pathname", "port", "auth", "query" };

           var url= Utility.UrlParse(@"http://www.foo.com/bar/123/abc", false, true, urlParsedFields);
            Assert.AreEqual(url.hostname, "www.foo.com");
            Assert.AreEqual(url.pathname, "/bar/123/abc");
            Assert.AreEqual(url.port, 80);
            Assert.AreEqual(url.protocol, "http");
        }

        //"ID=951357852456&FNAME=Jaime&LNAME=Lopez"

        [TestMethod]
        public void Utility_WhenPassQueryString_SerializeToJson()
        {
           var queryString = "ID=951357852456&FNAME=Jaime&LNAME=Lopez";

            var url = Utility.ParseQueryString(queryString);
            Assert.AreEqual(url, "{\"ID\":\"951357852456\",\"FNAME\":\"Jaime\",\"LNAME\":\"Lopez\"}");
        }
    }
}
