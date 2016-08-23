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
            public static Parser<AString> DirectiveName
            {
                get
                {
                    return AStringFromIdentifierChar(AlphaNumericIdentifierChar.Or(Underscore).Or(Dash).Or(ForwardSlash));
                }
            }

            public static Parser<AString>UnquotedDirectiveArg
            {
                get
                {
                    return
                        from a in AnyCharAString(" \'\r\n{};")
                        select a;

                }

            }

            public static Parser<AString> QuotedDirectiveArg
            {
                get
                {
                    return
                        from a in AnyCharAString("\'\r\n").Contained(SingleQuote, SingleQuote)
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
                        from directives in Directive.Or<IConfigurationNode>(DirectiveSection).Or(Comment).Many()
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
