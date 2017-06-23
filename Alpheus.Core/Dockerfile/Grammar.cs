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
    public partial class Dockerfile
    {
        public override Parser<ConfigurationTree<Instructions, InstructionNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<Instructions, InstructionNode> ParseTree(string f)
        {
            ConfigurationTree<Instructions, InstructionNode> tree = this.Parser.Parse(f);
            IEnumerable<XElement> ce = tree.Xml.Root.Descendants();
            foreach (XElement element in ce)
            {
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
            }
            return tree;
        }

        public class Grammar : Grammar<Dockerfile, Instructions, InstructionNode>
        {
            public static string[] ValidInstructionNames { get; } = { "ADD", "ARG",
                "CMD", "COPY", "ENTRYPOINT", "ENV", "EXPOSE", "FROM", "HEALTHCHECK", "LABEL",
                "MAINTAINER", "ONBUILD", "RUN", "SHELL", "STOPSIGNAL", "USER", "VOLUME", "WORKDIR" };
 
            public static Parser<AString> InstructionName
            {
                get
                {
                    return AStringFrom(AlphaNumericIdentifierChar);
                }
            }

            public static Parser<AString> InstructionValue
            {
                get
                {
                    return AnyCharExcept("\r\n");
                }
            }


           
            public static Parser<InstructionNode> Instruction
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from name in InstructionName
                            .Where(n => ValidInstructionNames.Contains(n.StringValue))
                        from s in Parse.WhiteSpace.AtLeastOnce()
                        from value in InstructionValue
                        select new InstructionNode(name, new List<AString> { value });

                }
            }


            public static Parser<InstructionCommentNode> Comment
            {
                get
                {
                    return
                        from w in OptionalMixedWhiteSpace
                        from c in Hash.Select(s => new AString { StringValue = new string(s, 1) }).Positioned()
                        from a in AnyCharExcept("\r\n").Optional()
                        select a.IsDefined ? new InstructionCommentNode(a.Get().Position.Line, a.Get()) : new InstructionCommentNode(c.Position.Line, c);
                }
            }

            public static Parser<Instructions> Instructions
            {
                get
                {
                    return
                        from i in Instruction.Or<IConfigurationNode>(Comment).Many()
                        select new Instructions(i);
                }
            }

            public static Parser<ConfigurationTree<Instructions, InstructionNode>> ConfigurationTree
            {
                get
                {
                    return Instructions.Select(s => new ConfigurationTree<Instructions, InstructionNode>("Container", s));
                }
            }
        }

    }
}
