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
        public AlpheusXPathVariable(string var_name)
        {
            name = var_name;
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
        #endregion

        #region Methods
        // This method is invoked at run time to find the value of the user defined variable.
        public object Evaluate(XsltContext xsltContext)
        {
            XsltArgumentList vars = ((AlpheusXsltContext)xsltContext).ArgList;
            return vars.GetParam(name, null);
        }
        #endregion

        #region Fields
        private string name;
        #endregion

    }
}
