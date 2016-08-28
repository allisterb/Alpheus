using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Xunit;
namespace Alpheus
{
    public partial class HttpdTests
    {
        [Fact]
        public void CanBuildXDocument()
        {

            XDocument x = httpd_2.ConfigurationTree.Xml;
            Assert.True(x.Declaration.Standalone == "yes");
            //Assert.True(x.Nodes().Count() > 1);
            x = httpd_3.ConfigurationTree.Xml;
            Assert.True(x.Nodes().Count() > 1);
        }

        [Fact]
        public void CanEvaluateBooleanXPath()
        {
            string e;
            List<string> result;
            bool r = httpd_1.ConfigurationTree.XPathEvaluate("//mysqld/user/Value='mysql'", out result, out e);
            Assert.True(r);
        }

        [Fact]
        public void CanEvaluateEnumerableXPath()
        {
            string e;
            List<string> result;
            bool r = httpd_1.ConfigurationTree.XPathEvaluate("/MySQL/mysqld/user", out result, out e);
            Assert.True(r);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void CanEvaluateMultipleXPath()
        {
            List<string> expressions = new List<string>() { "//mysqld/user/Value='mysql'", "//mysqld/port/Value='3307'" };
            Dictionary<string, Tuple<bool, List<string>, string>> results = httpd_1.ConfigurationTree.XPathEvaluate(expressions);
            Assert.True(results.Values.First().Item1);
            Assert.False(results.Values.Last().Item1);
            IConfiguration c = httpd_1;
            
        }

        [Fact]
        public void CanIncludeFiles()
        {
            var t = httpd_1.ParseTree(httpd_1.FileContents);
        }
    }
}
