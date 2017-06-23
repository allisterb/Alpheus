using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class MapDirectiveNode : DirectiveNode
    {
        public MapDirectiveNode(List<AString> values)  : base("Map", values) {}

     }
}
