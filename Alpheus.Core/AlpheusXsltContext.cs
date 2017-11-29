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
            this.Environment = env;
            this.Id = this.Environment.Rng.Next();
            this.AddNamespace("db", "urn:db");
            this.AddNamespace("os", "urn:os");
            this.AddNamespace("fs", "urn:fs");
            this.AddNamespace("ver", "urn:ver");
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
            return this.Environment.ResolveXPathFunction(prefix, name, ArgTypes);
        }

        // Function to resolve references to my AlpheusXslt variables.
        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return new AlpheusXPathVariable(prefix, name);
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

        public int Id { get; protected set; }

        public Dictionary<string, object> FunctionResults { get; protected set; } = new Dictionary<string, object>();
        #endregion

        #region Fields
        // XsltArgumentList to store my user defined variables
        private XsltArgumentList _ArgList;
        #endregion
    }

}
