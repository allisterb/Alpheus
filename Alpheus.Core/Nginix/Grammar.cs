using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
namespace Alpheus
{
    public partial class Nginx
    {
        public override Parser<ConfigurationTree<DirectiveSection, DirectiveNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<DirectiveSection, DirectiveNode> ParseTree(string f)
        {
            return this.Parser.Parse(f);
        }

        public class Grammar : Grammar<Nginx, DirectiveSection, DirectiveNode>
        {
            /*
            public static Parser<AString> Directive
            {
                get
                {
                    return AnyCharAString(" \"\r\n<>");
                    
                        from chars in Parse.AnyChar.Except(Parse.WhiteSpace.Or(DoubleQuote).Or(BeginEOL)
                            .Or(Parse.Char('>').Then(c => BeginEOL.Or(OpenAngledBracket)))
                            .Or(Parse.Char('<').Then(c => Parse.Char('/')))).Many().Text()
                            .Select(c => new AString(c)).Positioned()
                        select chars; 
                }
            }
            

            public static Parser<AString> AnySingleLineCharAStringW
            {
                get
                {
                    return
                        from chars in Parse.AnyChar.Except(DoubleQuote.Or(BeginEOL)
                            .Or(Parse.Char('>').Then(c => BeginEOL.Or(OpenAngledBracket)))
                            .Or(Parse.Char('<').Then(c => Parse.Char('/')))).Many().Text()
                            .Select(c => new AString(c)).Positioned()
                        select chars;
                }
            }

                            public static Parser<AString> StartDirectiveArg
            {
                get
                {
                    return
                        from o in Parse.WhiteSpace.AtLeastOnce()
                        from a in UnquotedDirectiveArg.XOr(QuotedDirectiveArg)
                        select a;
                }
            }



            public static Parser<DirectiveNode> StartDirective
            {
                get
                {
                    return
                        from n in DirectiveName
                        from v in DirectiveArg.Many().Optional()
                        select v.IsDefined ? new DirectiveNode(n, v.Get().ToList()) : new DirectiveNode(n);
                }
            }


            */

            public static Parser<AString> DirectiveName
            {
                get
                {
                    return AStringFromIdentifierChar(AlphaNumericIdentifierChar);
                }
            }

            public static Parser<AString>UnquotedDirectiveArg
            {
                get
                {
                    return
                        from a in AnyCharAString(" \"\r\n{};")
                        select a;

                }

            }

            public static Parser<AString> QuotedDirectiveArg
            {
                get
                {
                    return
                        from a in DoubleQuoted(AnyCharAString("\"\r\n"))
                        select a;
                }
            }

            public static Parser<AString> DirectiveArg
            {
                get
                {
                    return
                        from o in Parse.WhiteSpace.AtLeastOnce()
                        from a in UnquotedDirectiveArg.XOr(QuotedDirectiveArg)
                        select a;
                }
            }

            public static Parser<DirectiveNode> Directive
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from n in DirectiveName
                        from v in DirectiveArg.Many()
                        from sc in SemiColon
                        select new DirectiveNode(n, v.ToList());
                }
            }

            public static Parser<CommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in Hash.Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharAString("\"\r\n").Optional()
                        from sc in SemiColon
                        select a.IsDefined ? new CommentNode(a.Get().Position.Line, a.Get()) : new CommentNode(c.Position.Line, c);
                }
            }

            public static Parser<DirectiveNode> DirectiveSectionStart
            {
                get
                {
                    return
                        from n in Directive.Contained(OpenAngledBracket, ClosedAngleBracket)
                        select n;
                }
            }

            public static Parser<DirectiveSection> DirectiveSection
            {
                get
                {
                    return
                        from lw in OptionalMixedWhiteSpace
                        from s in DirectiveName
                        from w in OptionalMixedWhiteSpace
                        from ocb in OpenCurlyBracket
                        from d in Directive.Or<IConfigurationNode>(Comment).Or(DirectiveSection).Many()
                        from w2 in OptionalMixedWhiteSpace
                        from c in ClosedCurlyBracket
                        select new DirectiveSection(s, d);
                }
            }

            public static Parser<List<IConfigurationNode>> Directives
            {
                get
                {
                    return
                        from directives in Directive.Or<IConfigurationNode>(DirectiveSection).Many()
                        select directives.ToList();
                }
            }
            
            public static Parser<ConfigurationTree<DirectiveSection, DirectiveNode>> ConfigurationTree
            {
                get
                {
                    return Directives.Select(s => new ConfigurationTree<DirectiveSection, DirectiveNode>("Httpd", s));
                }
            }
        }

    }
}
