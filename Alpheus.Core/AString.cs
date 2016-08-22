using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sprache;

namespace Alpheus
{
    public class AString : IPositionAware<AString>
    {
        public AString() {}

        public AString(string value)
        {
            this.StringValue = value;
        }

        public AString SetPos(Position start, int length)
        {
            Position = start;
            Length = length;
            return this;
        }

        public Position Position {get; set; }

        public int Length { get; set; }

        public string StringValue {get; set; }

        public static implicit operator string(AString a)
        {
            return a.StringValue;
        }

        public static implicit operator AString(string s)
        {
            return new AString(s);
        }
        
        public static implicit operator XElement(AString a)
        {
            return new XElement(a.StringValue,
                new XAttribute[] {
                    new XAttribute("Position", a.Position.Pos), new XAttribute("Column", a.Position.Column), new XAttribute("Line", a.Position.Line),
                    new XAttribute("Length", a.Length)
                });
        }
    }
}
