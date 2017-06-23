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

        #endregion
    }
}
