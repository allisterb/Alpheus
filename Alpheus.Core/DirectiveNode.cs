using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class DirectiveNode : IConfigurationNode
    {
        public AString Name { get; set; }

        public List<AString> Values { get; set; }

        public bool IsTerminal { get; } = true;

        public DirectiveNode(AString name)
        {
            this.Name = name;
            this.Values = new List<AString>(0);
        }
        public DirectiveNode(AString name, List<AString> values)  
        {
            this.Name = name;
            this.Values = values;
            this.Values.RemoveAll(v => v.StringValue == string.Empty && v.Length == 0);
        }

        public static implicit operator XElement(DirectiveNode kv)
        {
            XElement x = kv.Name;
            foreach (AString v in kv.Values)
            {
                x.Add(new XElement("Arg",
                    new XAttribute[] {
                        new XAttribute("Position", v.Position.Pos), new XAttribute("Column", v.Position.Column),
                        new XAttribute("Line", v.Position.Line),
                        new XAttribute("Length", v.Length)
                }, v.StringValue));
            }
            return x;
        }
    }
}
