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
    public class TestEnvironment : AlpheusEnvironment
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
            XPathNodeIterator result = doc.CreateNavigator().Select("/root/fld");
            IEnumerable<int> func_entries = xslt_context.FunctionResults.Keys.Where(k => k.StartsWith(f.Prefix + "_"))
                .Select(k => k.Replace(f.Prefix + "_", string.Empty))
                .Select(k => Int32.Parse(k));
            int key;
            if (func_entries.Count() == 0)
            {
                key = 1;
            }
            else
            {
                key = func_entries.Max() + 1; 
            }
            xslt_context.FunctionResults.Add(f.Prefix + "_" + key.ToString(), result);
            return result;
        }

        public override object EvaluateXPathVariable(AlpheusXPathVariable v, AlpheusXsltContext xslt_context)
        {
            if (v.Prefix == "fs" && v.Name.StartsWith("_"))
            {
                string index = v.Name.Remove(0, 1);
                int k;
                if (Int32.TryParse(index, out k))
                {
                    string key = v.Prefix + v.Name;
                    if (xslt_context.FunctionResults.ContainsKey(key))
                    {
                        Debug("Resolved variable {0} from function result store.", key);
                        return xslt_context.FunctionResults[key];
                    }
                    else
                    {
                        return null;
                    }
                }
                else return null;
            }
            else return null;
        }
        #endregion

    }
}
