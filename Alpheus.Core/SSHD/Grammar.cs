using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
namespace Alpheus
{
    public partial class SSHD
    {
        public override Parser<ConfigurationTree<KeyValues, KeyValueNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<KeyValues, KeyValueNode> ParseTree(string f)
        {
            return this.Parser.Parse(f);
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
                    return Parse.AnyChar.Except(SingleQuote.Or(DoubleQuote).Or(Parse.Char('\n')).Or(Parse.Char('\r'))).Many().Text().Select(s => new AString(s)).Positioned();
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
                        from a in AnyCharAString.Optional()
                        select a.IsDefined ? new CommentNode(a.Get().Position.Line, a.Get()) : new CommentNode(c.Position.Line, c);
                }
            }

            public static Parser<KeyValues> Values
            {
                get
                {
                    return
                        from g1 in Key.Or(Comment).Many()
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
