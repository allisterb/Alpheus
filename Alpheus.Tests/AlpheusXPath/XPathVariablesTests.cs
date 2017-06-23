using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

using Xunit;
namespace Alpheus.Tests
{
    public partial class XPathVariablesTests
    {
        public SSHD sshd_1;

        public XPathVariablesTests()
        {
            sshd_1 = new SSHD(Path.Combine("SSHD", "sshd_config"), true);
        }

        [Fact]
        public void CanRegisterVariables()
        {
            XPathNavigator nav = sshd_1.XmlConfiguration.CreateNavigator();
            AlpheusXsltContext ctx = new AlpheusXsltContext(new LocalEnvironment());
            XPathExpression expr1 = nav.Compile("$db:foo");
            XPathExpression expr2 = nav.Compile("boolean(/SSHD)");
            expr1.SetContext(ctx);
            expr2.SetContext(ctx);
            object r = nav.Evaluate(expr1);
            r = nav.Evaluate(expr2);
        }
    }
}
