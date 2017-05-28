using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Alpheus
{
    public class AlpheusXsltContext : XsltContext
    {
        #region Constructors
        public AlpheusXsltContext(AlpheusEnvironment env) : base(new NameTable())
        {
            this.AddNamespace("db", "urn:db");
            this.AddNamespace("fs", "urn:fs");
            this.Environment = env;
        }

        public AlpheusXsltContext(NameTable nt, XsltArgumentList arg_list) : base(nt)
        {
            _ArgList = arg_list;
        }
        #endregion

        #region Overriden methods
        // Function to resolve references to my AlpheusXslt functions.
        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
        {
            if (prefix == "db")
            {
                // Create an instance of appropriate extension function class.
                switch (name)
                {
                    case "sql":
                        this.Environment.Debug("Resolved db:sql function.");
                        return new AlpheusXPathFunction("sql", 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.NodeSet);
                    default:
                        throw new ArgumentException("Unrecognized function: " + prefix + ":" + name);
                }
            }
            else return null;
        }

        // Function to resolve references to my AlpheusXslt variables.
        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return new AlpheusXPathVariable(name);
        }

        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return true;
        }

        public override bool Whitespace
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Properties
        // Returns the XsltArgumentList that contains AlpheusXslt variable definitions.
        public XsltArgumentList ArgList
        {
            get
            {
                return _ArgList;
            }
        }

        public AlpheusEnvironment Environment { get; protected set; }
        #endregion

        #region Fields
        // XsltArgumentList to store my user defined variables
        private XsltArgumentList _ArgList;
        #endregion
    }

}
