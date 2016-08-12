using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

using Sprache;

namespace Alpheus
{
    public class KeyValueSection : List<KeyValueNode>, IConfigurationNode
    {
        public AString Name { get; set; }

        public bool IsTerminal { get; set; } = false;

        public int CommentCount
        {
            get
            {
                return this.Where(kvn => kvn is CommentNode).Count();
            }
        }
        public KeyValueSection(AString name) : base(10)
        {
            this.Name = name;
        }

        public KeyValueSection(AString name, IEnumerable<KeyValueNode> keys) : this(name)
        {
            foreach (KeyValueNode k in keys)
            {
                if (k is CommentNode)
                {
                    k.Name = "Comment_" + (CommentCount + 1).ToString();
                }
                this.Add(k);
            }
        }

        public static implicit operator XElement(KeyValueSection s)
        {
            XElement x;
            if (s.Name == "global")
            {
                x = new XElement("Global");
            }
            else
            {
                x = new XElement(s.Name.StringValue,
                 new XAttribute[] {
                    new XAttribute("Position", s.Name.Position.Pos), new XAttribute("Column", s.Name.Position.Column), new XAttribute("Line", s.Name.Position.Line),
                    new XAttribute("Length", s.Name.Length)
                 });
            }
            foreach (KeyValueNode kv in s)
            {
                if (kv is CommentNode)
                {
                    CommentNode cn = kv as CommentNode;
                    x.Add((XElement)cn);
                }
                else
                {
                    x.Add((XElement)kv);
                }
            }
            return x;
        }
    }
}
