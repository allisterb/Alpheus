using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class DirectiveCommentNode : DirectiveNode
    {

        public AString Value { get; set; }
        public int Line
        {
            get
            {
                return this.Value.Position.Line;
            }
        }
        public DirectiveCommentNode(int line, AString value) : base("Line_" + line.ToString() + "_Comment", new List<AString> { value })
        {
            this.Value = value;
        }

        public static implicit operator XElement(DirectiveCommentNode c)
        {
            return new XElement(c.Name.StringValue, new XElement("Value",
             new XAttribute[] {
                    new XAttribute("Position", c.Value.Position.Pos), new XAttribute("Column", c.Value.Position.Column), new XAttribute("Line", c.Value.Position.Line),
                    new XAttribute("Length", c.Value.Length)
             }, c.Value.StringValue));
        }

    }
}
