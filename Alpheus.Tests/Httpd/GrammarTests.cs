using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using Xunit;

namespace Alpheus
{
    public partial class HttpdTests
    {
        [Fact]
        public void GrammarCanParseDirective()
        {
            string t = "ServerRoot \"C:/ Bitnami / wampstack - 5.6.18 - 0 / apache2\"";
            DirectiveNode d = Httpd.Grammar.Directive.Parse(t);
            Assert.Equal("ServerRoot", d.Name);
            t = "LoadModule access_compat_module modules/mod_access_compat.so";
            d = Httpd.Grammar.Directive.Parse(t);
            Assert.Equal("LoadModule", d.Name);
            Assert.Equal(2, d.Values.Count);
            d = Httpd.Grammar.Directive.Parse("CustomLog \"logs / access.log\" common");
            Assert.Equal("CustomLog", d.Name);
            Assert.Equal(2, d.Values.Count);
            Assert.Equal("logs / access.log", d.Values.First().StringValue);
        }

        [Fact]
        public void GrammarCanParseDirectiveSection()
        {
            string t = "<IfModule alias_module>" + Environment.NewLine +
                       "#" + Environment.NewLine +
                       "# Redirect: Allows you to tell clients about documents that used to " + Environment.NewLine +
                       "# exist in your server's namespace, but do not anymore. The client" + Environment.NewLine +
                       "</IfModule>";

            DirectiveSection ds = Httpd.Grammar.DirectiveSection.Parse(t);
            Assert.Equal("IfModule", ds.Name);
        }
    }
}
