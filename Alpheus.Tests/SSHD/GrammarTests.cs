using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using Xunit;

namespace Alpheus
{
    public partial class SSHDTests
    {
        [Fact]
        public void GrammarCanParseValues()
        {
            string t = Environment.NewLine + "# Package generated configuration file\n# See the sshd_config(5) manpage for details" + Environment.NewLine + "#ListenAddress ::" + Environment.NewLine + "#ListenAddress 0.0.0.0" + Environment.NewLine + "Protocol 2";
            KeyValues s = SSHD.Grammar.Values.Parse(t);
            Assert.Equal(5, s.Count);
            Assert.Equal(" Package generated configuration file", (s[0] as CommentNode).Value);
            Assert.Equal("2", (s[4] as KeyMultipleValueNode).Value.First());
            /*
            Assert.Equal(t.IndexOf("[section1]") + 1, s.Name.Position.Pos);
            Assert.Equal(2, s.Name.Position.Line);
            Assert.Equal(5, s.Name.Position.Column);
            Assert.Equal(t.IndexOf("key1"), s.First().Name.Position.Pos);
            Assert.Equal(t.IndexOf("value1"), s.First().Value.Position.Pos);
            Assert.Equal(t.IndexOf("key2"), s.Last().Name.Position.Pos);
            Assert.Equal(1, s.Last().Name.Position.Column);
            Assert.Equal(t.IndexOf("value2"), s.Last().Value.Position.Pos);
            Assert.Equal(6, s.Last().Value.Position.Column);
            t = "\r\r\n        [section2]   \nkey3 = value3\r\n\r\nkey4=value4";
            s = MySQL.Grammar.Section.Parse(t);
            Assert.Equal(2, s.Name.Position.Line);
            Assert.Equal(10, s.Name.Position.Column);
            Assert.Equal(t.IndexOf("key3"), s.First().Name.Position.Pos);
            Assert.Equal(t.IndexOf("value3"), s.First().Value.Position.Pos);
            Assert.Equal(t.IndexOf("key4"), s.Last().Name.Position.Pos);
            Assert.Equal(1, s.Last().Name.Position.Column);
            Assert.Equal(t.IndexOf("value4"), s.Last().Value.Position.Pos);
            Assert.Equal(6, s.Last().Value.Position.Column);
            */
        }
    }
}
