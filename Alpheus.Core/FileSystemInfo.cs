using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus.IO
{
    public abstract class FileSystemInfo : IFileSystemInfo
    {
        public string Name { get; protected set; }
        public string PathSeparator { get; protected set; }
        public string FullName { get; protected set; }
        public abstract bool Exists { get; protected set; }
        public IEnvironment Environment { get; protected set; }
        public FileSystemInfo(string file_path)
        {
            this.FullName = file_path;
        }
    }
}
