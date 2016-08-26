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
        public void GrammarCanParseQuotedIdentifier()
        {
            string t = "%h %l %u %t \\\"%Random\\\"";
            AString a = Httpd.Grammar.AnySingleCharAStringW.Parse(t);

        }

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
            d = Httpd.Grammar.Directive.Parse("CustomLog \"logs/access.log\" common");
            Assert.Equal("CustomLog", d.Name);
            Assert.Equal(2, d.Values.Count);
            Assert.Equal("logs/access.log", d.Values.First().StringValue);
            t = "LogFormat \" % h % l % u % t \"%r\" %>s %b \"%{Referer}i\" \"%{User-Agent}i\"\" combined";
            d = Httpd.Grammar.Directive.Parse(t);
        }

        [Fact]
        public void GrammarCanParseDirectiveName()
        {
            string t = "<IfVersion < 2.3 >\n";
            DirectiveNode d = Httpd.Grammar.DirectiveSectionStart.Parse(t);
            Assert.Equal("IfVersion", d.Name);
            Assert.Equal(2, d.Values.Count);
            Assert.Equal(d.Values[0].StringValue, "<");
            Assert.Equal(d.Values[1].StringValue, "2.3");
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
            Assert.Equal(3, ds.Count);
            t = "<IfModule alias_module>" + Environment.NewLine +
                       "#" + Environment.NewLine +
                       "# Redirect: Allows you to tell clients about documents that used to " + Environment.NewLine +
                       "# exist in your server's namespace, but do not anymore. The client" + Environment.NewLine +
                       "ScriptAlias /cgi-bin/ \"C:/Bitnami/wampstack-5.6.18-0/apache2/cgi-bin/\"" + Environment.NewLine +
                       "</IfModule>";
            ds = Httpd.Grammar.DirectiveSection.Parse(t);
            Assert.Equal("IfModule", ds.Name);
            Assert.Equal(4, ds.Count);
            Assert.True(ds[3] is DirectiveNode);
            DirectiveNode dn = ds[3] as DirectiveNode;
            Assert.Equal("ScriptAlias", dn.Name);
            Assert.Equal(2, dn.Values.Count);
        }

        [Fact]
        public void GrammarCanParseNestedDirectiveSection()
        {
            string t = "<IfModule headers_module>\nPHPIniDir \"C:/Bitnami/wampstack-5.6.18-0/php\"\n<IfVersion <2.3>\nLoadModule Foo\nScriptAlias /cgi-bin/\n</IfVersion></IfModule>";
            DirectiveSection ds = Httpd.Grammar.DirectiveSection.Parse(t);
            Assert.Equal("IfModule", ds.Start.Name);
            Assert.Equal("headers_module", ds.Start.Values[0]);
            DirectiveNode dn = ds.First() as DirectiveNode;
//            Assert.Equal("C:/Bitnami/wampstack-5.6.18-0/php", dn.Values[1]);
            DirectiveSection child = ds.First(n => n is DirectiveSection) as DirectiveSection;
            Assert.NotNull(child);
            Assert.Equal("IfVersion", child.Name);
        }
    }
}
