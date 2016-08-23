using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using Xunit;

namespace Alpheus
{
    public partial class NginxTests
    {
        string t = "user  www www;" + Environment.NewLine +
            "worker_processes  2;" + Environment.NewLine +
            "pid /var/run/nginx.pid;" + Environment.NewLine +
            "#       [ debug | info | notice | warn | error | crit ]" + Environment.NewLine +
            "error_log / ar/log/nginx.error_log  info;";
        string t2 = "events {\r\nworker_connections   2000;# use [ kqueue | epoll | /dev/poll | select | poll ];\r\nuse kqueue;\r\n}";
        string t3 = "events {\r\nworker_connections   2000;# use [ kqueue | epoll | /dev/poll | select | poll ];\r\nuse kqueue;\r\n"
            + "location / 404.html {\r\nroot  /spool/www;\r\n}proxy_buffers              4 32k;}";
       
        [Fact]
        public void GrammarCanParseDirective()
        {
            string[] ts = t.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            DirectiveNode d = Nginx.Grammar.Directive.Parse(ts[0]);
            Assert.Equal("user", d.Name);
            Assert.Equal(2, d.Values.Count);
            Assert.Equal("www", d.Values[0].StringValue);
            Assert.Equal("www", d.Values[1].StringValue);
            d = Nginx.Grammar.Directive.Parse(ts[1]);
            Assert.Equal("worker_processes", d.Name);
            Assert.Equal(1, d.Values.Count);
            Assert.Equal("2", d.Values[0].StringValue);
            d = Nginx.Grammar.Directive.Parse(ts[2]);
            Assert.Equal("pid", d.Name);
            Assert.Equal(1, d.Values.Count);
            Assert.Equal("/var/run/nginx.pid", d.Values[0].StringValue);
        }

        [Fact]
        public void GrammarCanParseComment()
        {
            string c = "# use [ kqueue | epoll | /dev/poll | select | poll ];\r\n";
            DirectiveCommentNode cn = Nginx.Grammar.Comment.Parse(c);
            Assert.True(cn.Value.StringValue.StartsWith(" use"));
        }

        [Fact]
        public void GrammarCanParseDirectiveSection()
        {
            DirectiveSection ds = Nginx.Grammar.DirectiveSection.Parse(t2);
            Assert.Equal("events", ds.Name);
            Assert.Equal(3, ds.Count);
            DirectiveNode dn = ds.Last() as DirectiveNode;
            Assert.Equal("use", dn.Name);
            Assert.Equal("kqueue", dn.Values[0]);
        }

        [Fact]
        public void GrammarCanParseNestedDirectiveSection()
        {
            DirectiveSection ds = Nginx.Grammar.DirectiveSection.Parse(t3);
            Assert.Equal(5, ds.Count);
            //Assert.Equal("headers_module", ds.Start.Values[0]);
            //DirectiveNode dn = ds.First() as DirectiveNode;
//            Assert.Equal("C:/Bitnami/wampstack-5.6.18-0/php", dn.Values[1]);
            //DirectiveSection child = ds.First(n => n is DirectiveSection) as DirectiveSection;
            //Assert.NotNull(child);
            //Assert.Equal("IfVersion", child.Name);
        }
    }
}
