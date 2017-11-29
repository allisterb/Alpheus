using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace Alpheus
{
    public class Commands : List<IConfigurationNode>, IConfigurationNode
    {
        public virtual AString Name { get; set; } = "Commands";

        public bool IsTerminal { get; set; } = false;

        public int CommentCount
        {
            get
            {
                return this.Where(kvn => kvn is CommandCommentNode).Count();
            }
        }
        public Commands() : base(1) { }

        public Commands(IEnumerable<IConfigurationNode> Commands) : this()
        {
            foreach (IConfigurationNode k in Commands)
            {
                if (k is CommandCommentNode)
                {
                    this.Add(k as CommandCommentNode);
                }
                else if (k is CommandNode)
                {
                    this.Add(k as CommandNode);
                }
                else throw new ArgumentOutOfRangeException("Commands", string.Format("Node {0} has an unknown type.", k.Name));
            }
        }

        public static implicit operator XElement(Commands Commands)
        {
            XElement x = new XElement("Commands");
            foreach (IConfigurationNode i in Commands)
            {
                if (i is CommandCommentNode)
                {
                    CommandCommentNode cn = i as CommandCommentNode;
                    x.Add((XElement)cn);
                }
                else if (i is CommandNode)
                {
                    CommandNode ins = i as CommandNode;
                    x.Add((XElement) ins);
                }
                else throw new ArgumentOutOfRangeException("Commands", string.Format("Node {0} has an unknown type.", i.Name));
            }
            return x;
        }
    }
}
