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
            string t = Environment.NewLine + "   [section1]" + Environment.NewLine + "key1=value1" + Environment.NewLine + "key2=value2";
            KeyValueSection s = MySQL.Grammar.Section.Parse(t);
            Assert.Equal(t.IndexOf("[section1]") + 1, s.Name.Position.Pos);
            Assert.Equal(2, s.Name.Position.Line);
            Assert.Equal(5, s.Name.Position.Column);
            Assert.Equal(t.IndexOf("key1"), s.First().Name.Position.Pos);
            KeyValueNode f = s.First() as KeyValueNode;
            Assert.Equal(t.IndexOf("value1"), f.Value.Position.Pos);
            Assert.Equal(t.IndexOf("key2"), s.Last().Name.Position.Pos);
            Assert.Equal(1, s.Last().Name.Position.Column);
            KeyValueNode l = s.Last() as KeyValueNode;
            Assert.Equal(t.IndexOf("value2"), l.Value.Position.Pos);
            Assert.Equal(6, l.Value.Position.Column);
            t = "\r\r\n        [section2]   \nkey3 = value3\r\n\r\nkey4=value4";
            s = MySQL.Grammar.Section.Parse(t);
            Assert.Equal(2, s.Name.Position.Line);
            Assert.Equal(10, s.Name.Position.Column);
            Assert.Equal(t.IndexOf("key3"), s.First().Name.Position.Pos);
            f = s.First() as KeyValueNode;
            Assert.Equal(t.IndexOf("value3"), f.Value.Position.Pos);
            Assert.Equal(t.IndexOf("key4"), (s.Last() as KeyValueNode).Name.Position.Pos);
            Assert.Equal(1, s.Last().Name.Position.Column);
            Assert.Equal(t.IndexOf("value4"), (s.Last() as KeyValueNode).Value.Position.Pos);
            Assert.Equal(6, (s.Last() as KeyValueNode).Value.Position.Column);
        }

        [Fact]
        public void GrammarCanParseComment()
        {
            string t = "  ;comment1\nkey2=value2";
            CommentNode comment1 = MySQL.Grammar.Comment.Parse(t);
            Assert.Equal(t.IndexOf("comment1"), comment1.Value.Position.Pos);
            //Assert.Equal("comment1", comment1.);
            t = "#\n";
            comment1 = MySQL.Grammar.Comment.Parse(t);
            Assert.True(comment1.Line == 1);
            comment1 = MySQL.Grammar.Comment.Parse("#\nkey1=value1");
            Assert.Equal(comment1.Value, string.Empty);
        }

        [Fact]
        public void GrammarCanParseSections()
        {
            List<IConfigurationNode> ct = MySQL.Grammar.Sections.Parse(my_2.FileContents).ToList();
            Assert.Equal(17, ct.Count(n => n is CommentNode));
            Assert.Equal(3, ct.Count(n => n is KeyValueSection));
            List<IConfigurationNode> ct2 = MySQL.Grammar.Sections.Parse(my_1.FileContents).ToList();
            //Assert.Equal(7, ct2.Count);
            //KeyValueSection mysqld = (ct2.Where(s => s.Name == "mysqld").First() as KeyValueSection);
            //Assert.Equal(57, mysqld.Where(kv => kv is CommentNode).Count());
            //Assert.True(mysqld.Any(kv => kv is KeyValueNode && kv.Name == "user" && ((KeyValueNode) kv).Value == "mysql"));
        }
    }
}
