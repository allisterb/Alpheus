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
        public ConfigurationTree(string root, S section)
        {
            XElement r = new XElement(root);
            if (section is KeyValues)
            {
                XElement e = section as KeyValues;
                r.Add(e);
            }
            else throw new ArgumentOutOfRangeException("No XElement conversion for section type available.");
            this.Xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), r);
        }

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
                else if (s is KeyValueSection)
                {
                    XElement e = s as DirectiveSection;
                    r.Add(e);
                }

                else throw new ArgumentOutOfRangeException("No XElement conversion for section type available.");
            }
            this.Xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), r);
        }

        public ConfigurationTree(string root, IEnumerable<IConfigurationNode> nodes)
        {
            XElement r = new XElement(root);
            foreach (IConfigurationNode n in nodes)
            {
                if (n is S && n is KeyValues)
                {
                    XElement e = n as KeyValues;
                    r.Add(e);
                }

                else if (n is S && n is KeyValueSection)
                {
                    XElement e = n as KeyValueSection;
                    r.Add(e);
                }
                else if (n is S && n is DirectiveSection)
                {
                    XElement e = n as DirectiveSection;
                    r.Add(e);
                }
                else if (n is V && n is KeyValueNode)
                {
                    XElement e = n as KeyValueNode;
                    r.Add(e);
                }
                else if (n is V && n is DirectiveCommentNode)
                {
                    XElement e = n as DirectiveCommentNode;
                    r.Add(e);
                }

                else if (n is V && n is DirectiveNode)
                {
                    XElement e = n as DirectiveNode;
                    r.Add(e);
                }
                else if (n is V && n is CommentNode)
                {
                    XElement e = n as CommentNode;
                    r.Add(e);
                }
                else throw new ArgumentOutOfRangeException("No XElement conversion for node type available.");
            }
            this.Xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), r);
        }

        public bool XPathEvaluate(string e, out List<string> result, out string message)
        {
            if (this.Xml == null) throw new InvalidOperationException("XML conversion for tree failed.");
            message = string.Empty;
            result = null;
            try
            {
                object r = this.Xml.XPathEvaluate(e);
                if (r as bool? != null)
                {
                    return ((bool?)r).Value;
                }
                else if (r as IEnumerable != null)
                {
                    result = new List<string>();
                    foreach (XObject xo in (IEnumerable) r)
                    {
                        if (xo is XElement)
                        {
                            result.Add((xo as XElement).ToString());
                        }
                        else if (xo is XAttribute)
                        {
                            result.Add((xo as XAttribute).ToString());
                        }
                    }
                    return result.Count > 0;
                }
                else if (r as double? != null)
                {
                    double? d = r as double?;
                    if (d.HasValue)
                    {
                        result = new List<string>(1) { d.Value.ToString() };
                        return true;
                    }
                    else return false;
                }
                else if (r as string != null)
                {
                    string s = r as string;
                    if (!string.IsNullOrEmpty(s))
                    {
                        result = new List<string>(1) { s };
                        return true;
                    }
                    else return false;
                }

                else
                {
                    return false;
                }
            }
            catch(XPathException xe)
            {
                this.LastXPathException = xe;
                message = xe.Message;
                return false;
            }
        }

        public Dictionary<string, Tuple<bool, List<string>, string>> XPathEvaluate(List<string> expressions)
        {
            Dictionary<string, Tuple<bool, List<string>, string>> results = new Dictionary<string, Tuple<bool, List<string>, string>>(expressions.Count);
            foreach (string e in expressions)
            {
                string message;
                List<string> result;
                bool r = this.XPathEvaluate(e, out result, out message);
                results.Add(e, new Tuple<bool, List<string>, string>(r, result, message));
            }
            return results;
        }
    }
}
