using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace Alpheus
{
    public partial class MySQL
    {
        public override Parser<ConfigurationTree<KeyValueSection, KeyValueNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<KeyValueSection, KeyValueNode> ParseTree(string f)
        {
            return this.Parser.Parse(f);
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
                    return Parse.AnyChar.Except(SingleQuote.Or(DoubleQuote).Or(Parse.Char('\n')).Or(Parse.Char('\r'))).Many().Text().Select(s => new AString(s)).Positioned();
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
                        from cb in CloseSquareBracket
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
                        from c in SemiColon.Or(Hash.Token()).Select(s => new AString { StringValue = new string(s, 1) }).Positioned().Token()
                        from a in AnyCharAString.Optional()
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
                        from ck in Comment.Or(Key).Many()
                        select new KeyValueSection(sn, ck);

                }
            }

            public static Parser<IEnumerable<KeyValueSection>> Sections
            {
                get
                {
                    return
                        from g1 in Key.Or(Comment).AtLeastOnce().Optional()
                        let global = g1.IsDefined ? new List<KeyValueSection> { new KeyValueSection("global", g1.Get()) } : null
                        from s in Section.Many()
                        select g1.IsDefined ? s.Concat(global) : s;
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
