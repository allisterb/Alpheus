using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using C5;

namespace Alpheus
{
    public class ConfigurationTree<S, V> where S: IConfigurationNode where V : IConfigurationNode 
    {
        XmlDocument Xml { get; set; }
        public void AddValue(S parent, V value)
        {
            //if (!this.Values.Contains(parent)) throw new ArgumentException(string.Format("The parent node {0} was not found.", parent.Name));
            
        }
    }
}
