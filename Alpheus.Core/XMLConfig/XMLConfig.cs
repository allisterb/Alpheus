using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Sprache;
using Alpheus.IO;

namespace Alpheus
{
    public class XMLConfig : ConfigurationFile<XMLConfigurationNode, XMLConfigurationNode>
    {
        #region Constructors
        public XMLConfig() : base() { }
        public XMLConfig(string file_path, bool read_file = true, bool parse_file = true) : base(file_path, string.Empty, read_file, parse_file) { }
        public XMLConfig(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<XMLConfigurationNode, XMLConfigurationNode>, string, string> read_file_lambda = null) : base(file, string.Empty, read_file, parse_file, read_file_lambda) { }
        #endregion

        #region Overriden methods
        public override ConfigurationFile<XMLConfigurationNode, XMLConfigurationNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<XMLConfigurationNode, XMLConfigurationNode>, string, string> read_file_lambda = null)
        {
            return new XMLConfig(file, read_file, parse_file, read_file_lambda);
        }

        public override Parser<ConfigurationTree<XMLConfigurationNode, XMLConfigurationNode>> Parser
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override ConfigurationTree<XMLConfigurationNode, XMLConfigurationNode> ParseTree(string f)
        {
            XDocument x = XDocument.Parse(f, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            foreach (XElement element in x.Root.Descendants())
            {
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
                IXmlLineInfo xli = element;
                if (element.Attribute("Line") == null && xli.HasLineInfo())
                {
                    element.Add(new XAttribute("Line", xli.LineNumber));
                }
            }
            return new ConfigurationTree<XMLConfigurationNode, XMLConfigurationNode>(x);
        }
        #endregion

    }
}
