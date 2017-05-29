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
        public AlpheusXPathFunction(string name, int min_args, int max_args, XPathResultType[] arg_types, XPathResultType return_type)
        {
            Name = name;
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
        #endregion

        #region Methods
        // This method is invoked at run time to execute the user defined function.
        public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {

            AlpheusXsltContext ctx = xsltContext as AlpheusXsltContext;
            return ctx.Environment.InvokeXPathFunction(this, ctx, args, docContext);
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
