using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus.IO
{
    public interface IEnvironment
    {
        #region Public properties
        bool IsWindows { get; }

        bool IsUnix { get; }

        bool IsMonoRuntime { get; }

        string PathSeparator { get; }

        string LineTerminator { get; }
        #endregion

        #region Public methods
        bool FileExists(string file_path);
        bool DirectoryExists(string dir_path);
        IFileInfo ConstructFile(string file_path);
        IDirectoryInfo ConstructDirectory(string dir_path);
        void Info(string message_format, params object[] message);
        void Status(string message_format, params object[] message);
        void Progress(string message_format, params object[] message);
        void Success(string message_format, params object[] message);
        void Warning(string message_format, params object[] message);
        void Debug(string message_format, params object[] message);
        void Error(string message_format, params object[] message);
        void Error(Exception e, string message_format, params object[] message);
        void Error(Exception e);
        void Error(AggregateException ae, string message_format, params object[] message);
        void Error(AggregateException ae);
        #endregion
    }
}
