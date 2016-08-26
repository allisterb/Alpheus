using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
namespace Alpheus
{
    public partial class Httpd
    {
        public override Parser<ConfigurationTree<DirectiveSection, DirectiveNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<DirectiveSection, DirectiveNode> ParseTree(string f)
        {
            return this.Parser.Parse(f);
        }

        //DoubleQuote.Then(q => Percent.Not()
        public class Grammar : Grammar<Httpd, DirectiveSection, DirectiveNode>
        {
            public static Parser<char> NotQuotedIdentifier
            {
                get
                {
                    return
                        from q in DoubleQuote
                        from n in Percent.Not()
                        select q;
                }
            }


            public static Parser<AString> AnySingleCharAString
            {
                get
                {
                    return
                        from chars in Parse.AnyChar.Except(Space.Or(DoubleQuote).Or(BeginEOL)
                            .Or(Parse.Char('>').Then(c => BeginEOL.Or(OpenAngledBracket)))
                            .Or(Parse.Char('<').Then(c => Parse.Char('/')))).Many().Text()
                            .Select(c => new AString(c)).Positioned()
                        select chars; 
                }
            }

            public static Parser<AString> AnySingleCharAStringW
            {
                get
                {
                    return
                        from chars in Parse.AnyChar.Except(NotQuotedIdentifier.Or(BeginEOL)).Many().Text()
                            .Select(c => new AString(c)).Positioned()
                        select chars;
                }
            }

            public static Parser<AString> QuotedEscapeSequence
            {
                get
                {
                    return
                        from q1 in DoubleQuote
                        from p in Percent
                        from s in AStringFromIdentifierChar(AlphaNumericIdentifierChar.Or(OpenCurlyBracket).Or(ClosedCurlyBracket))
                        from q2 in DoubleQuote
                        select s;
                }
            }

            public static Parser<AString> AnySingleLineCharAStringW2
            {
                get
                {
                    return
                        from s in AStringFromIdentifierChar(Parse.AnyChar.Except(BeginEOL.Or(DoubleQuote))).Or(QuotedEscapeSequence).Many()
                        select s.Aggregate((p, n) => p.StringValue += n.StringValue);
                }
            }


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
                        from a in AnySingleCharAString
                        select a;

                }

            }

            public static Parser<AString> QuotedDirectiveArg
            {
                get
                {
                    return
                        from q in DoubleQuote
                        from a in AnySingleCharAStringW
                        from q2 in DoubleQuote
                        select a;
                }
            }

            public static Parser<AString> DirectiveArg
            {
                get
                {
                    return
                        from s in Space.AtLeastOnce()
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

            public static Parser<DirectiveNode> Directive
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from n in DirectiveName
                        from v in DirectiveArg.Many()
                        select new DirectiveNode(n, v.ToList());
                }
            }

            public static Parser<DirectiveCommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in Hash.Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharAString("\r\n").Optional()
                        select a.IsDefined ? new DirectiveCommentNode(a.Get().Position.Line, a.Get()) : new DirectiveCommentNode(c.Position.Line, c);
                }
            }

            public static Parser<DirectiveNode> DirectiveSectionStart
            {
                get
                {
                    return
                        from n in StartDirective.Contained(OpenAngledBracket, ClosedAngleBracket)
                        select n;
                }
            }

            public static Parser<DirectiveSection> DirectiveSection
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from s in DirectiveSectionStart
                        from d in DirectiveSection.Or<IConfigurationNode>(Directive).Or(Comment).Many()
                        from w2 in OptionalMixedWhiteSpace
                        from o in OpenAngledBracket
                        from f in ForwardSlash
                        from cn in DirectiveName.Where(dn => dn.StringValue == s.Name.StringValue)
                        from c in ClosedAngleBracket
                        select new DirectiveSection(s, d);
                }
            }

            public static Parser<List<IConfigurationNode>> Directives
            {
                get
                {
                    return
                        from directives in DirectiveSection.Or<IConfigurationNode>(Directive).Or(Comment).Many()
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
