using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

using Sprache;
using Alpheus.IO;
namespace Alpheus
{
    public partial class Nginx
    {
        public override Parser<ConfigurationTree<DirectiveSection, DirectiveNode>> Parser { get; } = Grammar.ConfigurationTree;

        public class Grammar : Grammar<Nginx, DirectiveSection, DirectiveNode>
        {
            public static Parser<AString> DirectiveName
            {
                get
                {
                    return AStringFrom(AlphaNumericIdentifierChar.Or(Underscore).Or(Dash));
                }
            }

            public static Parser<AString>UnquotedDirectiveArg
            {
                get
                {
                    return
                        from a in AnyCharExcept(" \'\r\n{};#")
                        select a;

                }

            }

            public static Parser<AString> QuotedDirectiveArg
            {
                get
                {
                    return
                        from a in AnyCharExcept("\'\r\n").Contained(SingleQuote, SingleQuote)
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

            public static Parser<AString> MapDirectiveArg
            {
                get
                {
                    return
                        from o in Parse.WhiteSpace.AtLeastOnce().Optional()
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

            public static Parser<DirectiveNode> MapDirective
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from v in MapDirectiveArg.Many()
                        from sc in SemiColon
                        select new DirectiveNode(v.ToList());
                }
            }

            public static Parser<DirectiveCommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in Hash.Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharExcept("\r\n").Optional()
                        select a.IsDefined ? new DirectiveCommentNode(a.Get().Position.Line, a.Get()) : new DirectiveCommentNode(c.Position.Line, c);
                }
            }

            public static Parser<DirectiveNode> DirectiveSectionStart
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from n in DirectiveName
                        from a in DirectiveArg.Many().Optional()
                        from w2 in OptionalMixedWhiteSpace
                        from ocb in OpenCurlyBracket
                        select a.IsDefined ? new DirectiveNode(n, a.Get().ToList()) : new DirectiveNode(n);
                }
            }


            public static Parser<DirectiveSection> DirectiveSection
            {
                get
                {
                    return
                        from s in DirectiveSectionStart
                        from d in Directive.Or<IConfigurationNode>(MapDirective).Or(Comment).Or(DirectiveSection).Many()
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
                        from directives in Directive.Or<IConfigurationNode>(MapDirective).Or(DirectiveSection).Or(Comment).Many()
                        select directives.ToList();
                }
            }
            
            public static Parser<ConfigurationTree<DirectiveSection, DirectiveNode>> ConfigurationTree
            {
                get
                {
                    return Directives.Select(s => new ConfigurationTree<DirectiveSection, DirectiveNode>("Nginx", s));
                }
            }
        }

    }
}
