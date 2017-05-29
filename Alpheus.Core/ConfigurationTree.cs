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
        #region Constructors
        public ConfigurationTree(string root, S sections)
        {
            XElement r = new XElement(root);
            if (sections is KeyValues)
            {
                XElement e = sections as KeyValues;
                r.Add(e);
            }
            else
            {
                throw new ArgumentOutOfRangeException("No XElement conversion for section type available.");
            }
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
                else if (s is DirectiveSection)
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
                else if (n is V && n is KeyMultipleValueNode)
                {
                    XElement e = n as KeyMultipleValueNode;
                    r.Add(e);
                }
                else if (n is V && n is KeyValueNode)
                {
                    XElement e;
                    if (n is CommentNode)
                    {
                        e = n as CommentNode;
                        r.Add(e);
                    }
                    else
                    {
                        e = n as KeyValueNode;
                        r.Add(e);
                    }
                }
                else if (n is V && n is DirectiveNode)
                {
                    XElement e;
                    if (n is V && n is DirectiveCommentNode)
                    {
                        e = n as DirectiveCommentNode;
                        r.Add(e);
                    }
                    else
                    {
                        e = n as DirectiveNode;
                        r.Add(e);
                    }
                }
                else throw new ArgumentOutOfRangeException(string.Format("No XElement conversion for node {0} type available.", n.Name.StringValue));
            }
            this.Xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), r);
        }

        public ConfigurationTree(XDocument document)
        {
            this.Xml = document;
        }
        #endregion

        #region Properties
        public XDocument Xml { get; private set; }

        public XPathException LastXPathException { get; private set; }

        public ArgumentException LastArgumentException { get; private set; }

        public AlpheusEnvironment AlpheusEnvironment { get; set; }                                                                                                             
        #endregion

        #region Methods
        public bool XPathEvaluate(string e, out XElement result, out string message)
        {
            if (this.Xml == null) throw new InvalidOperationException("XML conversion for tree failed.");
            message = string.Empty;
            result = null;
            try
            {
                XPathNavigator nav = this.Xml.CreateNavigator();
                AlpheusXsltContext ctx = new AlpheusXsltContext(this.AlpheusEnvironment);
                XPathExpression xpe = nav.Compile(e);
                xpe.SetContext(ctx);
                object r = nav.Evaluate(xpe);
                if (r as bool? != null)
                {
                    return ((bool?)r).Value;
                }
                else if (r as IEnumerable != null)
                {
                    result = new XElement("Result");
                    XPathNodeIterator itr = r as XPathNodeIterator;
                    while (itr.MoveNext())
                    {
                        if (itr.Current.NodeType == XPathNodeType.Element)
                        {
                            result.Add(XElement.Load(itr.Current.ReadSubtree()));
                        }
                        else if (itr.Current.NodeType == XPathNodeType.Attribute)
                        {
                            result.Add(new XAttribute(itr.Current.Name, itr.Current.Value));
                        }
                        else if (itr.Current.NodeType == XPathNodeType.Text)
                        {
                            result.Add(new XText(itr.Current.Value));
                        }
                    }
                    return result.HasAttributes || result.HasElements;
                }
                else if (r as double? != null)
                {
                    double? d = r as double?;
                    if (d.HasValue)
                    {
                        result = new XElement("Result", d.Value.ToString());
                        return true;
                    }
                    else return false;
                }
                else if (r as string != null)
                {
                    string s = r as string;
                    if (!string.IsNullOrEmpty(s))
                    {
                        result = new XElement("Result", s); ;
                        return true;
                    }
                    else return false;
                }
                else
                {
                    return false;
                }
            }
            catch (XPathException xe)
            {
                this.LastXPathException = xe;
                message = xe.Message;
                return false;
            }
            catch (ArgumentException ae)
            {
                this.LastArgumentException = ae;
                message = ae.Message;
                return false;
            }
        }

        public bool XPathEvaluate(string e, out List<string> result, out string message)
        {
            if (this.Xml == null) throw new InvalidOperationException("XML conversion for tree failed.");
            message = string.Empty;
            result = null;
            try
            {
                XPathNavigator nav = this.Xml.CreateNavigator();
                AlpheusXsltContext ctx = new AlpheusXsltContext(this.AlpheusEnvironment);
                XPathExpression xpe = nav.Compile(e);
                xpe.SetContext(ctx);
                object r = nav.Evaluate(xpe);
                if (r as bool? != null)
                {
                    return ((bool?)r).Value;
                }
                else if (r as IEnumerable != null)
                {
                    result = new List<string>();
                    XPathNodeIterator itr = r as XPathNodeIterator;
                    while (itr.MoveNext())
                    {
                        if (itr.Current.NodeType == XPathNodeType.Element)
                        {
                            result.Add(XElement.Load(itr.Current.ReadSubtree()).ToString());
                        }
                        else
                        {
                            result.Add(itr.Current.Value);
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
            catch (ArgumentException ae)
            {
                this.LastArgumentException = ae;
                message = ae.Message;
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
        #endregion
    }
}
