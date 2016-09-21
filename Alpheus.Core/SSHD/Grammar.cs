using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

using Sprache;
namespace Alpheus
{
    public partial class SSHD
    {
        public override Parser<ConfigurationTree<KeyValues, KeyValueNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<KeyValues, KeyValueNode> ParseTree(string f)
        {
            ConfigurationTree<KeyValues, KeyValueNode> tree = this.Parser.Parse(f);
            IEnumerable<XElement> ce = tree.Xml.Root.Descendants();
            foreach (XElement element in ce)
            {
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
            }
            return tree;
        }

        public class Grammar : Grammar<SSHD, KeyValues, KeyValueNode>
        {
            public static Parser<AString> SectionNameAString
            {
                get
                {
                    return AStringFromIdentifierChar(AlphaNumericIdentifierChar);
                }
            }

            public static Parser<AString> KeyNameAString
            {
                get
                {
                    return AStringFromIdentifierChar(AlphaNumericIdentifierChar);
                }
            }

            public static Parser<AString> KeyValueAString
            {
                get
                {
                    return AnyCharAString("'\"\r\n");
                }

            }

            public static Parser<KeyValueNode> SingleValuedKey
            {
                get
                {
                    return
                        from k in KeyNameAString
                        from e in Parse.WhiteSpace.AtLeastOnce()
                        from v in KeyValueAString.Except(Parse.WhiteSpace.AtLeastOnce().Text())
                            .Or(DoubleQuoted(KeyValueAString))
                        select new KeyValueNode(k, v);
                }
            }

            public static Parser<KeyValueNode> Key
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from k in (SingleValuedKey)
                        select k;
                }
            }

            public static Parser<CommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in Hash.Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharAString("\r\n").Optional()
                        select a.IsDefined ? new CommentNode(a.Get().Position.Line, a.Get()) : new CommentNode(c.Position.Line, c);
                }
            }

            public static Parser<KeyValues> Values
            {
                get
                {
                    return
                        from g1 in Key.Or<IConfigurationNode>(Comment).Many()
                        select new KeyValues(g1);
                }
            }

            public static Parser<ConfigurationTree<KeyValues, KeyValueNode>> ConfigurationTree
            {
                get
                {
                    return Values.Select(s => new ConfigurationTree<KeyValues, KeyValueNode>("SSHD", s));
                }
            }
        }

    }
}
