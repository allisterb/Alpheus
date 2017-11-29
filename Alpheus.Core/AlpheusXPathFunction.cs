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
    public class AlpheusXPathFunction : IXsltContextFunction
    {
        #region Constructors
        public AlpheusXPathFunction(string prefix, string name, int min_args, int max_args, XPathResultType[] arg_types, XPathResultType return_type)
        {
            Name = name;
            Prefix = prefix;
            _MinArgs = min_args;
            _MaxArgs = max_args;
            _ArgTypes = arg_types;
            _ReturnType = return_type;
        }
        #endregion

        #region Properties
        public int Minargs
        {
            get
            {
                return _MinArgs;
            }
        }

        public int Maxargs
        {
            get
            {
                return _MaxArgs;
            }
        }

        public XPathResultType[] ArgTypes
        {
            get
            {
                return _ArgTypes;
            }
        }

        public XPathResultType ReturnType
        {
            get
            {
                return _ReturnType;
            }
        }

        public string Name { get; protected set; }

        public string Prefix { get; protected set; }
        #endregion

        #region Methods
        public object Invoke(XsltContext xslt_context, object[] args, XPathNavigator doc_context)
        {
            AlpheusXsltContext ctx = xslt_context as AlpheusXsltContext;
            return ctx.Environment.InvokeandStoreXPathFunction(this, ctx, args, doc_context);
        }
        #endregion

        #region Fields
        private XPathResultType[] _ArgTypes { get; set; }
        private XPathResultType _ReturnType { get; set; }
        
        private int _MinArgs;
        private int _MaxArgs;
        #endregion
    }
}
