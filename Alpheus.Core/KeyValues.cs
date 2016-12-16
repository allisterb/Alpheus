using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

using Sprache;

namespace Alpheus
{
    public class KeyValues : List<IConfigurationNode>, IConfigurationNode
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

        public KeyValues(IEnumerable<IConfigurationNode> keys) : this()
        {
            foreach (IConfigurationNode k in keys)
            {
                if (k is CommentNode)
                {
                    this.Add(k as CommentNode);
                }
                else if (k is KeyMultipleValueNode)
                {
                    this.Add(k as KeyMultipleValueNode);
                }
                else if (k is KeyValueNode)
                {
                    this.Add(k as KeyValueNode);
                }
                else throw new ArgumentOutOfRangeException("keys", string.Format("Key {0} has an unknown type.", k.Name));
            }
        }

        public static implicit operator XElement(KeyValues s)
        {
            XElement x = new XElement("Values");
            foreach (IConfigurationNode kv in s)
            {
                if (kv is CommentNode)
                {
                    CommentNode cn = kv as CommentNode;
                    x.Add((XElement)cn);
                }
                else if (kv is KeyMultipleValueNode)
                {
                    KeyMultipleValueNode kvn = kv as KeyMultipleValueNode;
                    x.Add((XElement)kvn);
                }
                else if (kv is KeyValueNode)
                {
                    KeyValueNode kvn = kv as KeyValueNode;
                    x.Add((XElement) kvn);
                }
                else throw new ArgumentOutOfRangeException("s", string.Format("Key {0} has an unknown type in section {1}.", kv.Name, s.Name));
            }
            return x;
        }
    }
}
