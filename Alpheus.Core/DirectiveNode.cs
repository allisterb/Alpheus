using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class DirectiveNode : IConfigurationNode
    {
        public AString Name { get; set; }

        public List<AString> Values { get; set; }

        public bool IsTerminal { get; } = true;

        public DirectiveNode(AString name, List<AString> values)  
        {
            this.Name = name;
            this.Values = values;
        }

        public static implicit operator XElement(DirectiveNode kv)
        {
            XElement x = new XElement(kv.Name.StringValue,
                new XAttribute[] {
                    new XAttribute("Position", kv.Name.Position.Pos), new XAttribute("Column", kv.Name.Position.Column), new XAttribute("Line", kv.Name.Position.Line),
                    new XAttribute("Length", kv.Name.Length)
                });
            foreach (AString v in kv.Values)
            {
                x.Add(new XElement("Value",
                    new XAttribute[] {
                        new XAttribute("Position", v.Position.Pos), new XAttribute("Column", v.Position.Column),
                        new XAttribute("Line", v.Position.Line),
                        new XAttribute("Length", v.Length)
                }));
            }
            return x;
        }
    }
}
