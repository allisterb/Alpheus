using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using Alpheus.IO;

namespace Alpheus
{
    public class LocalEnvironment : AlpheusEnvironment
    {
        #region Overriden methods
        public override bool FileExists(string file_path)
        {
            return File.Exists(file_path);
        }

        public override bool DirectoryExists(string dir_path)
        {
            return Directory.Exists(dir_path);
        }

        public override IFileInfo ConstructFile(string file_path)
        {
            return new LocalFileInfo(this, file_path);
        }

        public override IDirectoryInfo ConstructDirectory(string dir_path)
        {
            return new LocalDirectoryInfo(this, dir_path);
        }

        public override void Message(string message_type, string message_format, params object[] message)
        {
            Console.Write("[{0}] {1:HH: mm: ss}<{2,2:##}> ", message_type, DateTime.Now.ToUniversalTime(), Thread.CurrentThread.ManagedThreadId.ToString("D2"));
            Console.WriteLine(message_format, message);
        }

        public override IXsltContextFunction ResolveXPathFunction(string prefix, string name, XPathResultType[] ArgTypes)
        {
            if (prefix == "db")
            {
                switch (name)
                {
                    case "query":
                        Debug("Resolved db:query function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 2, new XPathResultType[] { XPathResultType.String, XPathResultType.String }, XPathResultType.NodeSet);
                    default:
                        Error("Unrecognized XPath function: {0}:{1}.", prefix, name);
                        return null;
                }
            }
            else if (prefix == "os")
            {
                switch (name)
                {
                    case "exec":
                        Debug("Resolved os:exec function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 2, new XPathResultType[] { XPathResultType.String }, XPathResultType.String);
                    default:
                        Error("Unrecognized XPath function: {0}:{1}.", prefix, name);
                        return null;
                }
            }
            else if (prefix == "ver")
            {
                switch (name)
                {
                    case "gt":
                        Debug("Resolved ver:gt function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.Boolean);
                    case "lt":
                        Debug("Resolved ver:lt function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.Boolean);
                    case "eq":
                        Debug("Resolved ver:eq function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.Boolean);
                    case "gte":
                        Debug("Resolved ver:gte function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.Boolean);
                    case "lte":
                        Debug("Resolved ver:gt function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.Boolean);
                    default:
                        Error("Unrecognized XPath function: {0}:{1}.", prefix, name);
                        return null;
                }
            }
            else if (prefix == "fs")
            {
                switch (name)
                {
                    case "exists":
                        Debug("Resolved fs:exists function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.Boolean);
                    case "text":
                        Debug("Resolved fs:text function.");
                        return new AlpheusXPathFunction(prefix, name, 1, 1, new XPathResultType[] { XPathResultType.String }, XPathResultType.String);
                    default:
                        Error("Unrecognized XPath function: {0}:{1}.", prefix, name);
                        return null;
                }
            }
            else return null;
        }
        public override object InvokeXPathFunction(AlpheusXPathFunction f, AlpheusXsltContext xslt_context, object[] args, XPathNavigator docContext)
        {
            
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml("<root><fld>val</fld><fld>val2</fld></root>");
            return doc.CreateNavigator().Select("/root/fld");
        }

        public override object EvaluateXPathVariable(AlpheusXPathVariable v, AlpheusXsltContext xslt_context)
        {
            return "varable";
        }
        #endregion

    }
}
