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
    public partial class Bash
    {
        public override Parser<ConfigurationTree<Commands, CommandNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<Commands, CommandNode> ParseTree(string f)
        {
            ConfigurationTree<Commands, CommandNode> tree = this.Parser.Parse(f);
            IEnumerable<XElement> ce = tree.Xml.Root.Descendants();
            foreach (XElement element in ce)
            {
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
            }
            return tree;
        }

        public class Grammar : Grammar<Bash, Commands, CommandNode>
        {

            public static Parser<AString> Word
            {
                get
                {
                    return AnyCharExcept("\r\n\"\' ");
                }
            }

            public static string[] BuiltinCommandNames { get; } = { "echo" };
 
            public static Parser<AString> CommandName
            {
                get
                {
                    return AStringFrom(AlphaNumericIdentifierChar);
                }
            }

            public static Parser<AString> MetaChar
            {
                get
                {
                    return AStringFrom(Parse.Char('$'));
                }
            }

            public static Parser<AString> DoubleQuotedLiteral
            {
                get
                {
                    return
                        from w in Parse.WhiteSpace.AtLeastOnce().Optional()
                        from v in DoubleQuoted(AnyCharExcept("\r\n\""))
                        select v;
                }
            }

            public static Parser<AString> SingleQuotedLiteral
            {
                get
                {
                    return
                        from w in Parse.WhiteSpace.AtLeastOnce().Optional()
                        from l in SingleQuoted(AnyCharExcept("\r\n\'"))
                        select l;
                }
            }

            public static Parser<AString> CommandArgValue
            {
                get
                {
                    return
                        from w in Parse.WhiteSpace.AtLeastOnce().Optional()
                        from v in DoubleQuoted(AnyCharExcept("\r\n\"")).Or(SingleQuoted(AnyCharExcept("\r\n\'"))).Or(AnyCharExcept("\r\n "))
                        select v;
                }
            }

 
            public static Parser<CommandNode> WordList            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from name in CommandName
                        from s in Parse.WhiteSpace.AtLeastOnce()
                        from value in CommandArgValue.DelimitedBy(Parse.WhiteSpace.Once().Optional())
                        select new CommandNode(name, value.ToList());

                }
            }


            public static Parser<CommandCommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in Hash.Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharExcept("\r\n").Optional()
                        select a.IsDefined ? new CommandCommentNode(a.Get().Position.Line, a.Get()) : new CommandCommentNode(c.Position.Line, c);
                }
            }

            public static Parser<Commands> Commands
            {
                get
                {
                    return
                        from i in WordList.Or<IConfigurationNode>(Comment).Many()
                        select new Commands(i);
                }
            }

            public static Parser<ConfigurationTree<Commands, CommandNode>> ConfigurationTree
            {
                get
                {
                    return Commands.Select(s => new ConfigurationTree<Commands, CommandNode>("Bash", s));
                }
            }
        }

    }
}
