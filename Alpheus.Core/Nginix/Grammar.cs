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
namespace Alpheus
{
    public partial class Nginx
    {
        public override Parser<ConfigurationTree<DirectiveSection, DirectiveNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<DirectiveSection, DirectiveNode> ParseTree(string f)
        {
            ConfigurationTree<DirectiveSection, DirectiveNode> tree = this.Parser.Parse(f);
            IEnumerable<XElement> ce = tree.Xml.Root.Descendants();
            foreach (XElement element in ce)
            {
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
            }

            object r = tree.Xml.XPathEvaluate("//include/Arg");
            if (r is IEnumerable)
            {
                IEnumerable results = r as IEnumerable;
                this.IncludeFiles = new List<Tuple<string, bool, ConfigurationFile<DirectiveSection, DirectiveNode>>>();
                foreach (XObject o in results)
                {
                    if (o is XElement)
                    {
                        XElement e = o as XElement;
                        string fn = e.Value;
                        if (!string.IsNullOrEmpty(fn))
                        {
                            fn = fn.Replace('/', Path.DirectorySeparatorChar);
                            if (System.IO.File.Exists(fn)) //try file path as absolute
                            {
                                Nginx conf = new Nginx(fn);
                                if (conf.ParseSucceded)
                                {
                                    IEnumerable<XElement> child_elements = conf.XmlConfiguration.Root.Descendants();
                                    foreach (XElement element in child_elements)
                                    {
                                        if (element.Attribute("File") == null) element.Add(new XAttribute("File", fn));
                                    }
                                    tree.Xml.Root.Add(child_elements);
                                }
                                this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<DirectiveSection, DirectiveNode>>
                                    (fn, conf.ParseSucceded, conf.ParseSucceded ? conf : null));
                            }
                            else
                            {
                                try
                                {
                                    FileInfo[] files = this.File.Directory.GetFiles(fn); //try relative to current file directory
                                    if (files != null && files.Count() > 0)
                                    {
                                        foreach (FileInfo file in files)
                                        {
                                            try
                                            {
                                                if (file.Exists)
                                                {
                                                    Nginx conf = new Nginx(file.FullName);
                                                    if (conf.ParseSucceded)
                                                    {
                                                        IEnumerable<XElement> child_elements = conf.XmlConfiguration.Root.Descendants();
                                                        foreach (XElement element in child_elements)
                                                        {
                                                            if (element.Attribute("File") == null) element.Add(new XAttribute("File", file.Name));
                                                        }
                                                        tree.Xml.Root.Add(child_elements);
                                                    }
                                                    this.IncludeFiles.Add(new Tuple<string, bool,
                                                        ConfigurationFile<DirectiveSection, DirectiveNode>>(file.Name, conf.ParseSucceded, conf.ParseSucceded ? conf : null));
                                                }
                                                else
                                                {
                                                    this.IncludeFiles.Add(new Tuple<string, bool,
                                                        ConfigurationFile<DirectiveSection, DirectiveNode>>(file.Name, false, null));
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                this.IncludeFiles.Add(new Tuple<string, bool,
                                                        ConfigurationFile<DirectiveSection, DirectiveNode>>(file.Name, false, null));
                                            }
                                        }
                                    }
                                    else //file doesn't exist
                                    {
                                        this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<DirectiveSection, DirectiveNode>>
                                        (fn, false, null));
                                    }

                                }
                                catch (Exception) //no luck trying to get a valid file path
                                {
                                    this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<DirectiveSection, DirectiveNode>>
                                                 (fn, false, null));
                                }
                            }
                        }
                    }
                }
            }
            return tree;
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
