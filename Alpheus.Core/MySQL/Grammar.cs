using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.Xml.XPath;
using Sprache;

namespace Alpheus
{
    public partial class MySQL
    {
        public override Parser<ConfigurationTree<KeyValueSection, KeyValueNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<KeyValueSection, KeyValueNode> ParseTree(string f)
        {
            ConfigurationTree<KeyValueSection, KeyValueNode> tree = this.Parser.Parse(f);
            IEnumerable<XElement> ce = tree.Xml.Root.Descendants();
            foreach (XElement element in ce)
            {
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
            }
            return tree;
        }

        public class Grammar : Grammar<MySQL, KeyValueSection, KeyValueNode>
        {
            public static Parser<AString> SectionNameAString
            {
                get
                {
                    return AStringFromIdentifierChar(AlphaNumericIdentifierChar.Or(Underscore));
                }
            }

            public static Parser<AString> KeyNameAString
            {
                get
                {
                    return AStringFromIdentifierChar(AlphaNumericIdentifierChar.Or(Underscore).Or(Dash));
                }
            }

            public static Parser<AString> KeyValueAString
            {
                get
                {
                    return AnyCharAString("'\"\r\n");
                }
              
            }
     
            public static Parser<AString> SectionName
            {
                get
                {
                    return
                        from w1 in OptionalMixedWhiteSpace
                        from ob in OpenSquareBracket
                        from sn in SectionNameAString
                        from cb in ClosedSquareBracket
                        select sn;
                }
            }

            public static Parser<KeyValueNode> SingleValuedKey
            {
                get
                {
                    return
                        from k in KeyNameAString
                        from e in Equal.Token()
                        from v in KeyValueAString
                        select new KeyValueNode(k, v);
                }
            }

            public static Parser<KeyValueNode> BooleanKey
            {
                get
                {
                    return
                        from k in KeyNameAString 
                        select new KeyValueNode(k, new AString { Length = 4, Position = k.Position, StringValue = "true" });
                }
            }

            public static Parser<KeyValueNode> MultiValuedKey
            {
                get
                {
                    return
                        from k in KeyNameAString
                        from e in Equal.Token()
                        from v in KeyValueAString.DelimitedBy(Comma)
                            .Select(value => new AString
                            {
                                Position = value.First().Position,
                                Length = value.Sum(l => l.Length),
                                StringValue = string.Join(",", value),
                            })
                        select new KeyValueNode(k, v);
                }
            }

            public static Parser<KeyValueNode> Key
            {
               get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from k in (SingleValuedKey).Or(MultiValuedKey).Or(BooleanKey)
                        select k;
                }
            }
            public static Parser<CommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in SemiColon.Or(Hash).Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharAString("\r\n").Optional()
                        select a.IsDefined ? new CommentNode(a.Get().Position.Line, a.Get()) : new CommentNode(c.Position.Line, c);
                }
            }

      
            public static Parser<KeyValueSection> Section
            {
                get
                {
                    return
                        from w1 in OptionalMixedWhiteSpace
                        from sn in SectionName
                        from ck in Key.Or<IConfigurationNode>(Comment).Many()
                        select new KeyValueSection(sn, ck);

                }
            }

            public static Parser<IEnumerable<IConfigurationNode>> Sections
            {
                get
                {
                    return
                        from s in Key.Or<IConfigurationNode>(Comment).Or(Section).Many()
                        select s;
                    
                }
            }

            public static Parser<ConfigurationTree<KeyValueSection, KeyValueNode>> ConfigurationTree
            {
                get
                {
                    return Sections.Select(s => new ConfigurationTree<KeyValueSection, KeyValueNode>("MySQL", s));
                }
            }
        }
    }

}
