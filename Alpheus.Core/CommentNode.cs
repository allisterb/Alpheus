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
        public CommentNode(AString name, AString value) : base(name, value) { } 
        
    }
}
