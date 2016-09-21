using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus.IO
{
    public class LocalFileInfo : IFileInfo
    {
        public IFileInfo Create (string file_path)
        {
            return new LocalFileInfo(file_path);
        }

        public string ReadAsText()
        {
            using (StreamReader s = new StreamReader(this.file.OpenRead()))
            {
                return s.ReadToEnd();
            }
        }

        public IDirectoryInfo Directory
        {
            get
            {
                return new LocalDirectoryInfo(this.file.Directory);
            }
        }

        public string Name
        {
            get
            {
                return this.file.Name;
            }
        }

        public string DirectoryName
        {
            get
            {
                return this.file.DirectoryName;
            }
        }

        public string FullName
        {
            get
            {
                return this.file.FullName;
            }
        }

        public bool Exists
        {
            get
            {
                return this.file.Exists;
            }
        }

        public long Length
        {
            get
            {
                return this.file.Length;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.file.IsReadOnly;
            }
        }

        public DateTime LastWriteTimeUtc
        {
            get
            {
                return this.file.LastAccessTimeUtc;
            }
        }

        public bool PathExists(string file_path)
        {
            return File.Exists(file_path);
        }

        public LocalFileInfo(string file_path)
        {
            this.file = new FileInfo(file_path);
        }

        public LocalFileInfo(FileInfo f)
        {
            this.file = f;
        }

       

        protected FileInfo file;
        protected IDirectoryInfo _Directory;
    }
}
