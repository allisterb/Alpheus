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

            public static Parser<AString> AnyKeyValue
            {
                get
                {
                    return AnyCharExcept("'\"\r\n");
                }
              
            }

            public static Parser<AString> BooleanTrueValue
            {
                get
                {
                    return
                         from v in AnyKeyValue
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
                         from v in AnyKeyValue
                         let s = v.StringValue.ToUpper()
                         where s == "OFF" || s == "FALSE" || s == "0" || s == "NO"
                         select new AString { Length = v.Length, Position = v.Position, StringValue = "false" };

                }

            }

            public static Parser<AString> KeyValue
            {
                get
                {
                    return BooleanTrueValue.Or(BooleanFalseValue).Or(AnyKeyValue);
                }

            }

            public static Parser<AString> QuotedKeyValue
            {
                get
                {
                    return DoubleQuoted(Optional(KeyValue)).Or(SingleQuoted(Optional(KeyValue)));
                }

            }

            

            public static Parser<KeyValueNode> SingleValuedKey
            {
                get
                {
                    return
                        from k in KeyName
                        from e in Equal.Token()
                        from v in KeyValue.Or(QuotedKeyValue)
                        select new KeyValueNode(k, v);
                }
            }

           
            public static Parser<KeyValueNode> IncludeFile
            {
                get
                {
                    return
                        from e in Exclamation
                        from k in AStringFrom(Parse.String("include"))
                        from s in Parse.WhiteSpace.AtLeastOnce()
                        from file in DoubleQuoted(KeyValue).Or(KeyValue)
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
                        from file in DoubleQuoted(KeyValue).Or(KeyValue)
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
                    return Values.Select(s => new ConfigurationTree<KeyValues, KeyValueNode>("PostgreSQL", s));
                }
            }
        }
    }

}
