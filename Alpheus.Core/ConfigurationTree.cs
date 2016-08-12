using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alpheus
{
    public class ConfigurationTree<S, V> where S: IConfigurationNode where V : IConfigurationNode 
    {
        XElement Tree { get; set; }
        public void AddValue(S parent, V value)
        {
            //if (Tree.Elements.)
            //if (!this.Values.Contains(parent)) throw new ArgumentException(string.Format("The parent node {0} was not found.", parent.Name));
            
        }

        public ConfigurationTree(string root)
        {
           Tree = new XElement(root); 
        }
    }
}
