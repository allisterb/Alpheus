using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Sprache;
using Newtonsoft.Json;

using Alpheus.IO;

namespace Alpheus
{
    public class JSONConfig : ConfigurationFile<JSONConfigurationNode, JSONConfigurationNode>
    {
        #region Constructors
        public JSONConfig() : base() { }
        public JSONConfig(string file_path, bool read_file = true, bool parse_file = true) : base(file_path, string.Empty, read_file, parse_file) { }
        public JSONConfig(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<JSONConfigurationNode, JSONConfigurationNode>, 
            string, string> read_file_lambda = null) 
            : base(file, string.Empty, new LocalEnvironment(), read_file, parse_file, read_file_lambda) { }
        public JSONConfig(IFileInfo file, AlpheusEnvironment env, bool read_file = true, bool parse_file = true, 
            Func<ConfigurationFile<JSONConfigurationNode, JSONConfigurationNode>, string, string> read_file_lambda = null)
            : base(file, string.Empty, env, read_file, parse_file, read_file_lambda) { }
        #endregion

        #region Overriden methods
        public override ConfigurationFile<JSONConfigurationNode, JSONConfigurationNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, 
            Func<ConfigurationFile<JSONConfigurationNode, JSONConfigurationNode>, string, string> read_file_lambda = null)
        {
            return new JSONConfig(file, this.AlEnvironment, read_file, parse_file, read_file_lambda);
        }

        public override Parser<ConfigurationTree<JSONConfigurationNode, JSONConfigurationNode>> Parser
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override ConfigurationTree<JSONConfigurationNode, JSONConfigurationNode> ParseTree(string f)
        {
            XDocument x;
            try
            {
                x = JsonConvert.DeserializeXNode(f, "Container");
            }
            catch (JsonSerializationException jse)
            {
                if (jse.Message == "XmlNodeConverter can only convert JSON that begins with an object. Path '', line 1, position 1.")
                {
                    x = JsonConvert.DeserializeXNode("{\"Container\":" + f + "}");
                }
                else throw;
            }
            foreach (XElement element in x.Root.DescendantsAndSelf())
            {
                if (element.Name.Namespace != XNamespace.None)
                {
                    element.Name = XNamespace.None.GetName(element.Name.LocalName);
                }
                if (element.Attributes().Where(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None).Any())
                {
                    element.ReplaceAttributes(element.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
                }
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
                IXmlLineInfo xli = element;
                if (element.Attribute("Line") == null && xli.HasLineInfo())
                {
                    element.Add(new XAttribute("Line", xli.LineNumber));
                }
            }
            return new ConfigurationTree<JSONConfigurationNode, JSONConfigurationNode>(x);
        }

        public override bool ReadFile()
        {
            bool r =  base.ReadFile();
            if (r)
            {
                string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                string t = this.FileContents;
                if (t.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
                {
                    var lastIndexOfUtf8 = _byteOrderMarkUtf8.Length;
                    this.FileContents = t.Remove(0, lastIndexOfUtf8);
                }
            }
            return r;
        }
        #endregion

    }
}
