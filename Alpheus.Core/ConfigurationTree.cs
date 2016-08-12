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
        public XDocument Xml { get; set; }
        public ConfigurationTree(string root, IEnumerable<S> sections)
        {
           XElement r = new XElement(root); 
           foreach (S s in sections)
            {
                if (s is KeyValueSection)
                {
                    XElement e = s as KeyValueSection;
                    r.Add(e);
                }
                else throw new ArgumentOutOfRangeException("No XElement conversion for section type available.");
            }
            this.Xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), r);
        }
    }
}
