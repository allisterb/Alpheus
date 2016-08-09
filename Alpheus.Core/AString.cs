using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
namespace Alpheus
{
    public class AString : ILensValue<AString>
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
    }
}
