using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml.XPath;

using Xunit;

namespace Alpheus.Tests
{
    public partial class XPathEnvironmentTests
    {
        public SSHD sshd_1;
        public TestEnvironment test_env = new TestEnvironment();
        public XPathEnvironmentTests()
        {
            sshd_1 = new SSHD(test_env.ConstructFile(Path.Combine("SSHD", "sshd_config")), test_env, true);
        }

        [Fact]
        public void CanStoreResult()
        {
            string message;
            List<string> results;
            bool result = sshd_1.XPathEvaluate("/SSHD/Values/Port", out results, out message, "1");
            Assert.True(sshd_1.AlEnvironment.Results.ContainsKey("1"));
            object r = sshd_1.AlEnvironment.Results["1"];
            Assert.True(r is XPathNodeIterator);
            XPathNodeIterator iter = r as XPathNodeIterator;
            Assert.True(iter.Count == 1);
        }

        [Fact]
        public void CanStoreFunctionResult()
        {
            string message;
            XElement result;
            bool r = sshd_1.XPathEvaluate("fs:exists('foo') and $fs:_1='exists'", out result, out message);
            //Assert.True(test_env.)
        }
    }
}
