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

        public class Grammar : Grammar<Httpd, DirectiveSection, DirectiveNode>
        {
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
                    return AStringFromIdentifierChar(AlphaNumericIdentifierChar.Or(Dot).Or(Underscore).Or(Dash));
                }

            }

            public static Parser<AString> QuotedDirectiveArg
            {
                get
                {
                    return DoubleQuoted(AStringFromIdentifierChar(Parse.AnyChar.Except(SingleQuote.Or(DoubleQuote).Or(Parse.Char('\n')).Or(Parse.Char('\r')))));
                }
            }

            public static Parser<AString> DirectiveArg
            {
                get
                {
                    return
                        from o in Parse.WhiteSpace.AtLeastOnce().Optional()
                        from a in UnquotedDirectiveArg.Or(QuotedDirectiveArg)
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
                        from a in AnyCharAString.Optional()
                        select a.IsDefined ? new CommentNode(a.Get().Position.Line, a.Get()) : new CommentNode(c.Position.Line, c);
                }
            }

            public static Parser<DirectiveNode> DirectiveSectionStart
            {
                get
                {
                    return
                        from o in OpenAngledBracket
                        from n in Directive
                        from c in ClosedAngleBracket
                        select n;
                }
            }

            public static Parser<DirectiveSection> DirectiveSection
            {
                get
                {
                    return
                        from s in DirectiveSectionStart
                        from d in Directive.Or<IConfigurationNode>(Comment).AtLeastOnce()
                        from w in OptionalMixedWhiteSpace
                        from o in OpenAngledBracket
                        from fs in ForwardSlash
                        from cn in DirectiveName.Where(dn => dn.StringValue == s.Name.StringValue)
                        from c in ClosedAngleBracket
                        select new DirectiveSection(s.Name, d);
                }
            }

            public static Parser<List<IConfigurationNode>> Directives
            {
                get
                {
                    return
                        from directives in Directive.Or<IConfigurationNode>(DirectiveSection).Many()
                        select directives.ToList();
                        //let gs = new DirectiveSection("GLOBAL", nodes.Where(s => s is DirectiveNode))
                        //let sections = nodes.Where(s => s is DirectiveSection).Select(s => s as DirectiveSection)
                        //let directives = new List<DirectiveSection>(sections.Count() + 1) { gs }
                        //select directives.Concat(sections).ToList();
                        //let os = sections.Where(s => s.Name.StringValue != "GLOBAL")
                        //select new List<DirectiveSection>(os.Count()) { new DirectiveSection("GLOBAL", gn) }
                        //.Concat(os).ToList();
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
