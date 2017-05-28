using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.Xml.XPath;
using CodePlex.XPathParser;

namespace Alpheus
{
    public class XPathAnalyzer
    {
        public XPathAnalyzer(string xpath)
        {
            var x = new XPathParser<XElement>();
            try
            {
                XElement t = x.Parse(xpath, new XPathStepsBuilder());
                if (t != null)
                {
                    this.Tree = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("xpath"));
                    this.Tree.Root.Add(t);
                    ParseSucceded = true;
                }
                else
                {
                    ParseSucceded = false;
                }
            }
            catch (Exception e)
            {
                LastException = e;
                ParseSucceded = false;
            }

        }

        #region Properties
        public Exception LastException { get; private set; }
        public XPathException LastXPathException { get; private set; }
        public bool ParseSucceded { get; private set; } = false;
        public XDocument Tree { get; protected set; }
        #endregion

        #region Methods
        public bool XPathEvaluate(string e, out XElement result, out string message)
        {
            if (this.Tree == null) throw new InvalidOperationException("XPath parsing failed.");
            message = string.Empty;
            result = null;
            try
            {
                object r = this.Tree.XPathEvaluate(e);
                if (r as bool? != null)
                {
                    return ((bool?)r).Value;
                }
                else if (r as IEnumerable != null)
                {
                    result = new XElement("Result");
                    foreach (XObject xo in (IEnumerable)r)
                    {
                        if (xo is XElement)
                        {
                            result.Add((xo as XElement));
                        }
                        else if (xo is XAttribute)
                        {
                            result.Add((xo as XAttribute));
                        }
                        else if (xo is XText)
                        {
                            result.Add((xo as XText));
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
        }

        public bool XPathEvaluate(string e, out List<string> result, out string message)
        {
            if (this.Tree == null) throw new InvalidOperationException("XML conversion for tree failed.");
            message = string.Empty;
            result = null;
            try
            {
                object r = this.Tree.XPathEvaluate(e);
                if (r as bool? != null)
                {
                    return ((bool?)r).Value;
                }
                else if (r as IEnumerable != null)
                {
                    result = new List<string>();
                    foreach (XObject xo in (IEnumerable)r)
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
            catch (XPathException xe)
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
            #endregion
        }
    }

}
