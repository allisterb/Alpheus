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
        public abstract void Info(string message_format, params object[] message);
        public abstract void Status(string message_format, params object[] message);
        public abstract void Progress(string message_format, params object[] message);
        public abstract void Success(string message_format, params object[] message);
        public abstract void Warning(string message_format, params object[] message);
        public abstract void Debug(string message_format, params object[] message);
        public abstract void Error(string message_format, params object[] message);
        public abstract void Error(Exception e, string message_format, params object[] message);
        public abstract void Error(Exception e);
        public abstract object InvokeXPathFunction(AlpheusXPathFunction f, XsltContext xsltContext, object[] args, XPathNavigator docContext);
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
    }
}
