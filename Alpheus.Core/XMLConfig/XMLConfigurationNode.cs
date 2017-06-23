using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus
{
    public class XMLConfigurationNode : XElement, IConfigurationNode
    {
        AString IConfigurationNode.Name { get; set; }

        bool IConfigurationNode.IsTerminal { get; } = false;

        public XMLConfigurationNode(AString name) : base(name) { }
    }
}
