using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

using Sprache;

namespace Alpheus
{
    public class KeyValues : List<KeyValueNode>, IConfigurationNode
    {
        public AString Name { get; set; } = "Values";

        public bool IsTerminal { get; set; } = false;

        public int CommentCount
        {
            get
            {
                return this.Where(kvn => kvn is CommentNode).Count();
            }
        }
        public KeyValues() : base(1) {}

        public KeyValues(IEnumerable<KeyValueNode> keys) : this()
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

        public static implicit operator XElement(KeyValues s)
        {
            XElement x = new XElement("Values");
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
