using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class KeyMultipleValueNode : IConfigurationNode
    {
        public AString Name { get; set; }

        public bool IsTerminal { get; } = true;

        public List<AString> Value { get; set; }

        public KeyMultipleValueNode(AString name, IEnumerable<AString> value)
        {
            this.Name = name;
            this.Value = value.ToList();
        }

        public static implicit operator XElement(KeyMultipleValueNode kv)
        {
            XElement x =  new XElement(kv.Name.StringValue,
                new XAttribute[] {
                    new XAttribute("Position", kv.Name.Position.Pos), new XAttribute("Column", kv.Name.Position.Column), new XAttribute("Line", kv.Name.Position.Line),
                    new XAttribute("Length", kv.Name.Length)
                });
            for (int i = 0; i < kv.Value.Count; i++)
            {
                AString value = kv.Value[i];
                XElement v = new XElement("Value_" + (i + 1).ToString(),
                new XAttribute[] {
                    new XAttribute("Position", value.Position.Pos), new XAttribute("Column", value.Position.Column), new XAttribute("Line", value.Position.Line),
                    new XAttribute("Length", value.Length)
                });
                v.SetValue(value.StringValue);
                x.Add(v);
            }
            return x;
        }
    }
}
