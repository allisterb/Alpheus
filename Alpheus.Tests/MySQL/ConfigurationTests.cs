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
    public partial class MySQLTests
    {
        [Fact]
        public void CanBuildXDocument()
        {
            XDocument x = my_1.ConfigurationTree.Xml;
            Assert.True(x.Declaration.Standalone == "yes");
        }

        [Fact]
        public void CanEvaluateBooleanXPath()
        {
            string e;
            List<string> result;
            bool r = my_1.ConfigurationTree.XPathEvaluate("//mysqld/user/Value='mysql'", out result, out e);
            Assert.True(r);
        }

        [Fact]
        public void CanEvaluateEnumerableXPath()
        {
            string e;
            List<string> result;
            bool r = my_1.ConfigurationTree.XPathEvaluate("/MySQL/mysqld/user", out result, out e);
            Assert.True(r);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void CanEvaluateMultipleXPath()
        {
            List<string> expressions = new List<string>() { "//mysqld/user/Value='mysql'", "//mysqld/port/Value='3307'" };
            Dictionary<string, Tuple<bool, List<string>, string>> results = my_1.ConfigurationTree.XPathEvaluate(expressions);
            Assert.True(results.Values.First().Item1);
            Assert.False(results.Values.Last().Item1);
            IConfiguration c = my_1;
            
        }
    }
}
