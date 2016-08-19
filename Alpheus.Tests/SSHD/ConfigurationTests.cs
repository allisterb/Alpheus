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
    public partial class SSHDTests
    {
        [Fact]
        public void CanBuildXDocument()
        {

            XDocument x = sshd_1.ConfigurationTree.Xml;
            Assert.True(x.Declaration.Standalone == "yes");
        }

        [Fact]
        public void CanEvaluateBooleanXPath()
        {
            string e;
            List<string> result;
            bool r = sshd_1.ConfigurationTree.XPathEvaluate("/SSHD/Values/HostKey='/etc/ssh/ssh_host_rsa_key'", out result, out e);
            Assert.True(r);
        }

        [Fact]
        public void CanEvaluateEnumerableXPath()
        {
            string e;
            List<string> result;
            bool r = sshd_1.ConfigurationTree.XPathEvaluate("/SSHD/Values/HostKey", out result, out e);
            Assert.True(r);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void CanEvaluateMultipleXPath()
        {
            List<string> expressions = new List<string>() { "/SSHD/Values/UsePAM='yes'", "/SSHD/Values/X11Forwarding='yes'" };
            Dictionary<string, Tuple<bool, List<string>, string>> results = sshd_1.ConfigurationTree.XPathEvaluate(expressions);
            Assert.True(results.Values.First().Item1);
            Assert.True(results.Values.Last().Item1);
            IConfiguration c = sshd_1;
            
        }
    }
}
