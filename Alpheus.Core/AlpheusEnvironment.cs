using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using Alpheus.IO;

namespace Alpheus
{
    public abstract class AlpheusEnvironment
    {
        #region Abstract methods
        public abstract bool FileExists(string file_path);
        public abstract bool DirectoryExists(string dir_path);
        public abstract IFileInfo ConstructFile(string file_path);
        public abstract IDirectoryInfo ConstructDirectory(string dir_path);
        public abstract void Message(string message_type, string message_format, params object[] message);
        public abstract IXsltContextFunction ResolveXPathFunction(string prefix, string name, XPathResultType[] ArgTypes);
        public abstract object InvokeXPathFunction(AlpheusXPathFunction f, AlpheusXsltContext xslt_context, object[] args, XPathNavigator doc_context);
        public abstract object EvaluateXPathVariable(AlpheusXPathVariable v, AlpheusXsltContext xslt_context);
        #endregion

        #region Properties
        public bool IsWindows
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT;
            }
        }

        public bool IsUnix
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Unix;
            }
        }

        public bool IsMonoRuntime
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }

        public string LineTerminator
        {
            get
            {
                return this.IsWindows ? "\r\n" : "\n";
            }
        }

        public string PathSeparator
        {
            get
            {
                return this.IsWindows ? "\\" : "/";
            }
        }

        public Random Rng { get; } = new Random();

        public Dictionary<string, object> Results = new Dictionary<string, object>();
        #endregion

        #region Methods
        public virtual void Debug(string message_format, params object[] message)
        {
            Message("DEBUG", string.Format(message_format, message));
        }

        public virtual void Info(string message_format, params object[] message)
        {
            Message("INFO", string.Format(message_format, message));
        }

        public virtual void Success(string message_format, params object[] message)
        {
            Message("SUCCESS", string.Format(message_format, message));
        }

        public virtual void Warning(string message_format, params object[] message)
        {
            Message("WARNING", string.Format(message_format, message));
        }

        public virtual void Progress(string message_format, params object[] message)
        {
            Message("PROGRESS", string.Format(message_format, message));
        }

        public virtual void Status(string message_format, params object[] message)
        {
            Message("STATUS", string.Format(message_format, message));
        }

        public virtual void Error(Exception e)
        {
            Message("ERROR", "Exception: {0} {1}", e.Message, e.StackTrace);
        }

        public virtual void Error(Exception e, string message_format, params object[] message)
        {
            Message("ERROR", "Exception: {0} {1}", e.Message, e.StackTrace, string.Format(message_format, message));
        }

        public virtual void Error(string message_format, params object[] message)
        {
            Message("ERROR", string.Format(message_format, message));
        }

        public virtual object InvokeandStoreXPathFunction(AlpheusXPathFunction f, AlpheusXsltContext xslt_context, object[] args, XPathNavigator doc_context)
        {
            object result = this.InvokeXPathFunction(f, xslt_context, args, doc_context);
            IEnumerable<int> stored_func_results = xslt_context.FunctionResults.Keys.Where(k => k.StartsWith(f.Prefix + "_"))
                .Select(k => k.Replace(f.Prefix + "_", string.Empty))
                .Select(k => Int32.Parse(k));
            int key;
            if (stored_func_results.Count() == 0)
            {
                key = 1;
            }
            else
            {
                key = stored_func_results.Max() + 1;
            }
            xslt_context.FunctionResults.Add(f.Prefix + "_" + key.ToString(), result);
            return result;
        }

        public virtual object RetrieveOrEvaluateXPathVariable(AlpheusXPathVariable v, AlpheusXsltContext xslt_context)
        {
            if (xslt_context.HasNamespace(v.Prefix) && v.Name.StartsWith("_"))
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
                        return EvaluateXPathVariable(v, xslt_context);
                    }
                }
                else return EvaluateXPathVariable(v, xslt_context);
            }
            else return EvaluateXPathVariable(v, xslt_context);
        }
        #endregion
    }
}
