using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class KeyValueNode : IConfigurationNode
    {
        public AString Name { get; set; }

        public AString Value { get; set; }

        public bool IsTerminal { get; } = true;

        

        public KeyValueNode(AString name, AString value)  
        {
            this.Name = name;
            this.Value = value;
        }

        public static implicit operator XElement(KeyValueNode kv)
        {
            XElement x = new XElement(kv.Name);
            XElement v = kv.Value;
            x.AddFirst(v);
            return x;
        }
    }
}
