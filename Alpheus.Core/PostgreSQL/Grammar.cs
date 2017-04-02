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
    public partial class PostgreSQL
    {
        public override Parser<ConfigurationTree<KeyValues, KeyValueNode>> Parser { get; } = Grammar.ConfigurationTree;

        public class Grammar : Grammar<PostgreSQL, KeyValues, KeyValueNode>
        {
            
            public static Parser<AString> KeyName
            {
                get
                {
                    return AStringFrom(AlphaNumericIdentifierChar.Or(Underscore).Or(Dash));
                }
            }

            public static Parser<AString> AnyUnquotedKeyValue
            {
                get
                {
                    return AStringFrom(AlphaNumericIdentifierChar.Or(Underscore));
                }
              
            }

            public static Parser<AString> AnyQuotedKeyValue
            {
                get
                {
                    return AnyCharExcept("'\r\n");
                }

            }

            public static Parser<AString> BooleanTrueValue
            {
                get
                {
                    return
                         from v in AnyUnquotedKeyValue
                         let s = v.StringValue.ToUpper()
                         where s == "ON" || s == "TRUE" || s == "1" || s == "YES"
                         select new AString { Length = v.Length, Position = v.Position, StringValue = "true" };
                    
                }

            }

            public static Parser<AString> BooleanFalseValue
            {
                get
                {
                    return
                         from v in AnyUnquotedKeyValue
                         let s = v.StringValue.ToUpper()
                         where s == "OFF" || s == "FALSE" || s == "0" || s == "NO"
                         select new AString { Length = v.Length, Position = v.Position, StringValue = "false" };

                }

            }

            public static Parser<AString> UnquotedKeyValue
            {
                get
                {
                    return BooleanTrueValue.Or(BooleanFalseValue).Or(AnyUnquotedKeyValue);
                }

            }

            public static Parser<AString> QuotedKeyValue
            {
                get
                {
                    return SingleQuoted(Optional(AnyQuotedKeyValue));
                }

            }

            

            public static Parser<KeyValueNode> SingleValuedKey
            {
                get
                {
                    return
                        from k in KeyName
                        from e in Equal.Token()
                        from v in UnquotedKeyValue.Or(QuotedKeyValue)
                        select new KeyValueNode(k, v);
                }
            }

           
            public static Parser<KeyValueNode> IncludeFile
            {
                get
                {
                    return
                        from e in Exclamation
                        from k in AStringFrom(Parse.String("include_if_exists").Or(Parse.String("include")))
                        from s in Parse.WhiteSpace.AtLeastOnce()
                        from file in SingleQuoted(AnyQuotedKeyValue).Or(AnyUnquotedKeyValue)
                        select new KeyValueNode(k, file);
                }
            }

            public static Parser<KeyValueNode> IncludeDir
            {
                get
                {
                    return
                        from e in Exclamation
                        from k in AStringFrom(Parse.String("include_dir"))
                        from s in Parse.WhiteSpace.AtLeastOnce()
                        from file in SingleQuoted(AnyQuotedKeyValue).Or(AnyUnquotedKeyValue)
                        select new KeyValueNode(k, file);
                }
            }

            public static Parser<KeyValueNode> IncludeKey
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from k in (IncludeFile).Or(IncludeDir)
                        select k;
                }
            }

            public static Parser<KeyValueNode> Key
            {
               get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from k in (SingleValuedKey).Or(IncludeKey)
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

            public static Parser<ConfigurationTree<KeyValues, KeyValueNode>> ConfigurationTree
            {
                get
                {
                    return Values.Select(s => new ConfigurationTree<KeyValues, KeyValueNode>("PGSQL", s));
                }
            }
        }
    }

}
