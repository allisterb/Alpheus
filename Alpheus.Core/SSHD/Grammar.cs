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
        public override Parser<ConfigurationTree<KeyValues, KeyMultipleValueNode>> Parser { get; } = Grammar.ConfigurationTree;

        public class Grammar : Grammar<SSHD, KeyValues, KeyMultipleValueNode>
        {
            public static Parser<AString> KeyName
            {
                get
                {
                    return AStringFrom(AlphaNumericIdentifierChar);
                }
            }

            public static Parser<AString> UnquotedKeyValue
            {
                get
                {
                    return AnyCharExcept("'\"\r\n ");
                }
            }


            public static Parser<AString> QuotedKeyValue
            {
                get
                {
                    return DoubleQuoted(AnyCharExcept("'\"\r\n"));
                }

            }

            public static Parser<IEnumerable<AString>> KeyValue
            {
                get
                {
                    return QuotedKeyValue.Or(UnquotedKeyValue).DelimitedBy(Comma.Or(Space));
                }

            }

            public static Parser<KeyMultipleValueNode> Key
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from k in KeyName
                        from s in Parse.WhiteSpace.AtLeastOnce()
                        from v in KeyValue
                        select new KeyMultipleValueNode(k, v);

                }
            }

            public static Parser<CommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in Hash.Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharExcept("\r\n").Optional()
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

            public static Parser<ConfigurationTree<KeyValues, KeyMultipleValueNode>> ConfigurationTree
            {
                get
                {
                    return Values.Select(s => new ConfigurationTree<KeyValues, KeyMultipleValueNode>("SSHD", s));
                }
            }
        }

    }
}
