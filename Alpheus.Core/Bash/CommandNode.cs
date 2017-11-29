using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class CommandNode : IConfigurationNode
    {
        #region Constructors
        public CommandNode(AString name, List<AString> arguments)
        {
            this.Name = name;
            this.Arguments = arguments;

        }
        #endregion

        #region Properties
        public AString Name { get; set; }

        public List<AString> Arguments { get; set; }

        public bool IsTerminal { get; } = true;
        #endregion

        #region Operators
        public static implicit operator XElement(CommandNode kv)
        {
            XElement x = kv.Name;
            foreach (AString v in kv.Arguments)
            {
                x.Add(new XElement("Arg",
                    new XAttribute[] {
                        new XAttribute("Position", v.Position.Pos), new XAttribute("Column", v.Position.Column),
                        new XAttribute("Line", v.Position.Line),
                        new XAttribute("Length", v.Length)
                }, v.StringValue));
            }
            return x;
        }
        #endregion

    }
}
