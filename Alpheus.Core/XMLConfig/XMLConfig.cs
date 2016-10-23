using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Sprache;
using Alpheus.IO;

namespace Alpheus
{
    public class XMLConfig : ConfigurationFile<XMLConfigurationNode, XMLConfigurationNode>
    {
        #region Constructors
        public XMLConfig() : base() { }
        public XMLConfig(string file_path, bool read_file = true, bool parse_file = true) : base(file_path, read_file, parse_file) { }
        public XMLConfig(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<XMLConfigurationNode, XMLConfigurationNode>, string, string> read_file_lambda = null) : base(file, read_file, parse_file, read_file_lambda) { }
        #endregion

        public override Parser<ConfigurationTree<XMLConfigurationNode, XMLConfigurationNode>> Parser
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override ConfigurationTree<XMLConfigurationNode, XMLConfigurationNode> ParseTree(string f)
        {
            return new ConfigurationTree<XMLConfigurationNode, XMLConfigurationNode>(XDocument.Parse(f, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo));
         }
    }
}
