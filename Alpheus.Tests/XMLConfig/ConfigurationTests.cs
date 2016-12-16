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
    public partial class XMLConfigTests
    {
        [Fact]
        public void CanBuildXDocument()
        {

            XDocument x = xmlconfig_1.ConfigurationTree.Xml;
            Assert.True(x.Declaration.Encoding == "utf-8");
        }

        [Fact]
        public void CanEvaluateBooleanXPath()
        {
            string e;
            List<string> result;
            bool r = xmlconfig_1.ConfigurationTree.XPathEvaluate("boolean(/configuration/connectionStrings)", out result, out e);
            Assert.True(r);
        }

        [Fact]
        public void CanEvaluateEnumerableXPath()
        {
            
            string e;
            List<string> result;
            bool r = xmlconfig_1.ConfigurationTree.XPathEvaluate("/configuration/connectionStrings", out result, out e);
            Assert.True(r);
            Assert.True(result.Count > 0);
            
        }

        [Fact]
        public void CanEvaluateMultipleXPath()
        {
            /*
            List<string> expressions = new List<string>() { "/SSHD/Values/UsePAM='yes'", "/SSHD/Values/X11Forwarding='yes'" };
            Dictionary<string, Tuple<bool, List<string>, string>> results = sshd_1.ConfigurationTree.XPathEvaluate(expressions);
            Assert.True(results.Values.First().Item1);
            Assert.True(results.Values.Last().Item1);
            IConfiguration c = sshd_1;
            */
        }
    }
}
