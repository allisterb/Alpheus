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
                    k.Name = "Comment #" + (CommentCount + 1).ToString();
                }
                this.Add(k);
            }
        }

        public static implicit operator XElement(KeyValueSection s)
        {
            XElement x = s.Name;
            foreach (KeyValueNode kv in s)
            {
                x.Add((XElement)kv);
            }
            return x;
        }
    }
}
