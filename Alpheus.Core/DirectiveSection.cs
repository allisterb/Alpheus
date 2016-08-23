using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

using Sprache;

namespace Alpheus
{
    public class DirectiveSection : List<IConfigurationNode>, IConfigurationNode
    {
        public AString Name { get; set; }

        public bool IsTerminal { get; set; } = false;

        public DirectiveNode Start { get; set;}

        public int CommentCount
        {
            get
            {
                return this.Where(kvn => kvn is DirectiveCommentNode).Count();
            }
        }
        public DirectiveSection(AString name) : base(10)
        {
            this.Name = name;
        }

        public DirectiveSection(AString name, IEnumerable<IConfigurationNode> children) : this(name)
        {
            foreach (IConfigurationNode n in children)
            {
                if (n is DirectiveSection || n is DirectiveNode || n is DirectiveCommentNode)
                {
                    if (n is DirectiveCommentNode)
                    {
                        n.Name = "Comment_" + (CommentCount + 1).ToString();
                    }
                    this.Add(n);
                }
                else throw new ArgumentOutOfRangeException(string.Format("Child {0} is not a directive section, directive node or comment.", n.Name));
            }
        }

        public DirectiveSection(DirectiveNode start, IEnumerable<IConfigurationNode> children) : this(start.Name, children)
        {
            this.Start = start;
        }

        public static implicit operator XElement(DirectiveSection s)
        {
            XElement x = s.Start == null ? (XElement)s.Name : (XElement)s.Start;
            foreach (IConfigurationNode n in s)
            {
                if (n is DirectiveCommentNode)
                {
                    DirectiveCommentNode cn = n as DirectiveCommentNode;
                    x.Add((XElement)cn);
                }
                else if (n is DirectiveSection)
                {
                    DirectiveSection ds = n as DirectiveSection;
                    x.Add((XElement)ds);
                }
                else if (n is DirectiveNode)
                {
                    DirectiveNode dn = n as DirectiveNode;
                    x.Add((XElement)dn);
                }
                else throw new ArgumentOutOfRangeException(string.Format("Child {0} is not a directive section, directive node or comment.", n.Name));
            }
            return x;
        }
    }
}
