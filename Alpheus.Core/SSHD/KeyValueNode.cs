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

        public static KeyValueNode operator + (KeyValueNode left, KeyValueNode right)
        {
            left.Value.StringValue = left.Value.StringValue + " " + right.Value.StringValue;
            left.Value.Length = left.Value.Length + 1 + right.Value.Length;
            return left;
        }

        public static implicit operator XElement(KeyValueNode kv)
        {
            XElement x =  new XElement(kv.Name.StringValue,
                new XAttribute[] {
                    new XAttribute("Position", kv.Name.Position.Pos), new XAttribute("Column", kv.Name.Position.Column), new XAttribute("Line", kv.Name.Position.Line),
                    new XAttribute("Length", kv.Name.Length)
                }, 
                new XElement("Value",
                new XAttribute[] {
                    new XAttribute("Position", kv.Value.Position.Pos), new XAttribute("Column", kv.Value.Position.Column), new XAttribute("Line", kv.Value.Position.Line),
                    new XAttribute("Length", kv.Value.Length)
                }));
            XElement v = x.FirstNode as XElement;
            v.SetValue(kv.Value.StringValue);
            return x;
        }
    }
}
