using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using Xunit;

namespace Alpheus
{
    public partial class MySQLTests
    {
        [Fact]
        public void GrammarCanParseSection()
        {
            string t = "\n   [section1]\nkey1=value1\nkey2=value2";
            KeyValueSection s = MySQL.Grammar.Section.Parse(t);
            Assert.Equal(t.IndexOf("[section1]") + 1, s.Name.Position.Pos);
            Assert.Equal(2, s.Name.Position.Line);
            Assert.Equal(5, s.Name.Position.Column);
            Assert.Equal(t.IndexOf("key1"), s.First().Key.Position.Pos);
            Assert.Equal(t.IndexOf("value1"), s.First().Value.Position.Pos);
            Assert.Equal(t.IndexOf("key2"), s.Last().Key.Position.Pos);
            Assert.Equal(1, s.Last().Key.Position.Column);
            Assert.Equal(t.IndexOf("value2"), s.Last().Value.Position.Pos);
            Assert.Equal(6, s.Last().Value.Position.Column);
            t = "\r\r\n        [section2]   \nkey3 = value3\r\n\r\nkey4=value4";
            s = MySQL.Grammar.Section.Parse(t);
            Assert.Equal(2, s.Name.Position.Line);
            Assert.Equal(10, s.Name.Position.Column);
            Assert.Equal(t.IndexOf("key3"), s.First().Key.Position.Pos);
            Assert.Equal(t.IndexOf("value3"), s.First().Value.Position.Pos);
            Assert.Equal(t.IndexOf("key4"), s.Last().Key.Position.Pos);
            Assert.Equal(1, s.Last().Key.Position.Column);
            Assert.Equal(t.IndexOf("value4"), s.Last().Value.Position.Pos);
            Assert.Equal(6, s.Last().Value.Position.Column);
        }

        [Fact]
        public void GrammarCanParseComment()
        {
            string t = "  ;comment1\nkey2=value2";
            AString comment1 = MySQL.Grammar.Comment.Parse(t);
        }
    }
}
