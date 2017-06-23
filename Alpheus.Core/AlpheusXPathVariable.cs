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
    public class AlpheusXPathVariable : IXsltContextVariable
    {
        #region Constructors
        public AlpheusXPathVariable(string prefix, string var_name)
        {
            Prefix = prefix;
            Name = var_name;
        }
        #endregion

        #region Properties
        public bool IsLocal
        {
            get
            {
                return false;
            }
        }

        public bool IsParam
        {
            get
            {
                return false;
            }
        }

        public XPathResultType VariableType
        {
            get
            {
                return XPathResultType.Any;
            }
        }

        public string Name { get; protected set; }
        public string Prefix { get; protected set; }
        #endregion

        #region Methods
        // This method is invoked at run time to find the value of the user defined variable.
        public object Evaluate(XsltContext xslt_context)
        {
            AlpheusXsltContext ctx = xslt_context as AlpheusXsltContext;
            XsltArgumentList vars = ctx.ArgList;
            return ctx.Environment.EvaluateXPathVariable(this, ctx);
        }
        #endregion

    }
}
