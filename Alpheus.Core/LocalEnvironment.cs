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

        public override void Debug(string message_format, params object[] message)
        {
            Message("DEBUG", string.Format(message_format, message));
        }

        public override void Info(string message_format, params object[] message)
        {
            Message("INFO", string.Format(message_format, message));
        }

        public override void Success(string message_format, params object[] message)
        {
            Message("SUCCESS", string.Format(message_format, message));
        }

        public override void Warning(string message_format, params object[] message)
        {
            Message("WARNING", string.Format(message_format, message));
        }

        public override void Progress(string message_format, params object[] message)
        {
            Message("PROGRESS", string.Format(message_format, message));
        }

        public override void Status(string message_format, params object[] message)
        {
            Message("STATUS", string.Format(message_format, message));
        }

        public override void Error(Exception e)
        {
            Message("ERROR", "Exception: {0} {1}", e.Message, e.StackTrace);
        }

        public override void Error(Exception e, string message_format, params object[] message)
        {
            Message("ERROR", "Exception: {0} {1}", e.Message, e.StackTrace, string.Format(message_format, message));
        }

        public override void Error(string message_format, params object[] message)
        {
            Message("ERROR", string.Format(message_format, message));
        }

        public override object InvokeXPathFunction(AlpheusXPathFunction f, XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {
            
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml("<root><fld>val</fld><fld>val2</fld></root>");
            return doc.CreateNavigator().Select("/root/fld");
        }
        #endregion

        #region Methods
        public void Message(string message_type, string message_format, params object[] message)
        {
            Console.Write("[{0}] {1:HH: mm: ss}<{2,2:##}> ", message_type, DateTime.Now.ToUniversalTime(), Thread.CurrentThread.ManagedThreadId.ToString("D2"));
            Console.WriteLine(message_format, message);
        }
        #endregion
    }
}
