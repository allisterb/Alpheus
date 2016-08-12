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
        XDocument Tree { get; set; }
        public ConfigurationTree(string root, IEnumerable<S> sections)
        {
           XElement r = new XElement(root); 
           foreach (S s in sections)
            {
                if (s is KeyValueSection)
                {
                    r.Add(s as KeyValueSection);
                }
                else throw new ArgumentOutOfRangeException("No XElement conversion for section type available.");
            }
            Tree = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), r);
        }
    }
}
