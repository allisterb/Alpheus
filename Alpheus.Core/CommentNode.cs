using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class CommentNode : KeyValueNode
    {
        public CommentNode(int line, AString value) : base("Line_" + line.ToString() + "_Comment", value) { }

        public static implicit operator XElement(CommentNode c)
        {
            return new XElement(c.Name.StringValue, new XElement("Value",
             new XAttribute[] {
                    new XAttribute("Position", c.Value.Position.Pos), new XAttribute("Column", c.Value.Position.Column), new XAttribute("Line", c.Value.Position.Line),
                    new XAttribute("Length", c.Value.Length)
             }, new XElement("Value", c.Value.StringValue)));
        }

    }
}
