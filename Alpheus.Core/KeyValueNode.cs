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
        public AString Name
        {
            get
            {
                return this.KeyValue.Key;
            }
            set
            {
                this.KeyValue = new KeyValuePair<AString, AString>(value, this.KeyValue.Value);
            }
        }

        public bool HasChildren { get; } = false;

        private KeyValuePair<AString, AString> KeyValue;

        public KeyValueNode(AString name, AString value)  
        {
            this.Name = name;
            this.KeyValue = new KeyValuePair<AString, AString>(name, value);
        }

        public static implicit operator KeyValuePair<AString, AString> (KeyValueNode kv)
        {
            return kv.KeyValue;
        }

        public static implicit operator KeyValueNode(KeyValuePair<AString, AString> kv)
        {
            return new KeyValueNode(kv.Key, kv.Value);
        }

        public static implicit operator XElement(KeyValueNode kv)
        {
            XElement x = new XElement(kv.Name);
            XElement v = kv.KeyValue.Value;
            x.AddFirst(v);
            return x;
        }
    }
}
