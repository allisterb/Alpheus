using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

using Sprache;

namespace Alpheus
{
    public class KeyValueSection : Dictionary<AString, AString>, IConfigurationNode
    {
        public AString Name { get; set; }

        public bool IsTerminal { get; set; } = false;

        public KeyValueSection(AString name) : base(10)
        {
            this.Name = name;
        }

        public KeyValueSection(AString name, IEnumerable<KeyValueNode> keys) : this(name)
        { 
            foreach(KeyValuePair<AString, AString> k in keys)
            {
                this.Add(k.Key, k.Value);
            }
        }

        public static implicit operator XElement(KeyValueSection s)
        {
            XElement x = s.Name;
            foreach (KeyValueNode kv in s)
            {
                x.Add((XElement)kv);
            }
            return x;
        }
    }
}
