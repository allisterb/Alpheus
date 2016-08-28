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
    public partial class Httpd
    {
        public override Parser<ConfigurationTree<DirectiveSection, DirectiveNode>> Parser { get; } = Grammar.ConfigurationTree;

        public override ConfigurationTree<DirectiveSection, DirectiveNode> ParseTree(string f)
        {
            ConfigurationTree<DirectiveSection, DirectiveNode> tree = this.Parser.Parse(f);
            object r = tree.Xml.XPathEvaluate("/Httpd/Include/Arg | /Httpd/IncludeOptional/Arg");
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
                                Httpd conf = new Httpd(fn);
                                if (conf.ParseSucceded)
                                {
                                    tree.Xml.Root.Add(conf.XmlConfiguration.Root.Descendants());
                                }
                                this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<DirectiveSection, DirectiveNode>>
                                    (fn, conf.ParseSucceded, conf));
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
                                            if (file.Exists)
                                            {
                                                Httpd conf = new Httpd(File.FullName);
                                                if (conf.ParseSucceded)
                                                {
                                                    tree.Xml.Root.Add(conf.XmlConfiguration.Root.Descendants());
                                                }
                                                this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<DirectiveSection, DirectiveNode>>
                                                    (file.Name, conf.ParseSucceded, conf));
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
       
        public class Grammar : Grammar<Httpd, DirectiveSection, DirectiveNode>
        {
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

            //Thanks to jay for solution for parsing quoted delimiters: http://stackoverflow.com/a/33464588
            public static Parser<AString> QuotedDirectiveArg ()
            {
                    Parser<AString> escaped_delimiter = AStringFromString(Parse.String("\\\"").Text().Named("Escaped delimiter"));
                    Parser<AString> single_escape = AStringFromString(Parse.String("\\").Text()).Named("Single escape character");
                    Parser<AString> double_escape = AStringFromString(Parse.String("\\\\").Text()).Named("Escaped escape character");
                    Parser<AString> delimiter  = AStringFromIdentifierChar(Parse.Char('"').Named("Delimiter"));
                    Parser<AString> simple_literal = AStringFromString(Parse.AnyChar.Except(single_escape).Except(delimiter).Many().Text()).Named("Literal without escape/delimiter character");

                    return 
                        from start in delimiter
                        from v in escaped_delimiter.Or(double_escape).Or(single_escape).Or(simple_literal).Many()
                        from end in delimiter
                        select v.Aggregate((p, n) => p + n);
                              
            }

            public static Parser<AString> DirectiveArg
            {
                get
                {
                    return
                        from s in Space.AtLeastOnce()
                        from a in QuotedDirectiveArg().XOr(UnquotedDirectiveArg)
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
