using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace Alpheus
{
    public class Instructions : List<IConfigurationNode>, IConfigurationNode
    {
        public virtual AString Name { get; set; } = "Instructions";

        public bool IsTerminal { get; set; } = false;

        public int CommentCount
        {
            get
            {
                return this.Where(kvn => kvn is InstructionCommentNode).Count();
            }
        }
        public Instructions() : base(1) { }

        public Instructions(IEnumerable<IConfigurationNode> instructions) : this()
        {
            foreach (IConfigurationNode k in instructions)
            {
                if (k is InstructionCommentNode)
                {
                    this.Add(k as InstructionCommentNode);
                }
                else if (k is InstructionNode)
                {
                    this.Add(k as InstructionNode);
                }
                else throw new ArgumentOutOfRangeException("instructions", string.Format("Node {0} has an unknown type.", k.Name));
            }
        }

        public static implicit operator XElement(Instructions instructions)
        {
            XElement x = new XElement("Instructions");
            foreach (IConfigurationNode i in instructions)
            {
                if (i is InstructionCommentNode)
                {
                    InstructionCommentNode cn = i as InstructionCommentNode;
                    x.Add((XElement)cn);
                }
                else if (i is InstructionNode)
                {
                    InstructionNode ins = i as InstructionNode;
                    x.Add((XElement) ins);
                }
                else throw new ArgumentOutOfRangeException("instructions", string.Format("Node {0} has an unknown type.", i.Name));
            }
            return x;
        }
    }
}
