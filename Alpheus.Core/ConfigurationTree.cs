using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Alpheus
{
    public class ConfigurationTree<S, V> where S: IConfigurationNode where V : IConfigurationNode 
    {
        public XDocument Xml { get; private set; }

        public XPathException LastXPathException { get; private set; }
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

        public bool XPathEvaluate(string e, out IEnumerable result, out string message)
        {
            if (this.Xml == null) throw new InvalidOperationException("XML conversion for tree failed.");
            message = string.Empty;
            result = null;
            try
            {
                result = (IEnumerable) this.Xml.XPathEvaluate(e);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    int count = 0;
                    foreach (XObject on in result)
                    {
                        count++;
                    }
                    if (count == 0)
                    {
                        
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch(XPathException xe)
            {
                this.LastXPathException = xe;
                message = xe.Message;
                return false;
            }
        }
    }
}
