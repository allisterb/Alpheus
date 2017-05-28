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
        #region Constructors
        public LocalFileInfo(string file_path)
        {
            this.file = new FileInfo(file_path);
            this.Name = file.Name;
            this.FullName = file.FullName;
        }

        public LocalFileInfo(AlpheusEnvironment env, string file_path) : this(file_path)
        {
            this.Environment = env;
        }

        public LocalFileInfo(FileInfo f) 
        {
            this.file = f;
            this.Name = file.Name;
            this.FullName = file.FullName;
        }

        public LocalFileInfo(AlpheusEnvironment env, FileInfo f) : this(f)
        {
            this.Environment = env;
        }
        #endregion

        #region Overriden properties
        public string PathSeparator
        {
            get
            {
                return this._PathSeparator;
            }
        }

        public IDirectoryInfo Directory
        {
            get
            {
                return new LocalDirectoryInfo(this.file.Directory);
            }
        }

        public string DirectoryName
        {
            get
            {
                return this.file.DirectoryName;
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

        public string FullName { get; protected set; }


        public string Name { get; protected set; }


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

        public AlpheusEnvironment Environment { get; private set; }
        #endregion

        #region Overriden methods
        public string ReadAsText()
        {
            using (StreamReader s = new StreamReader(this.file.OpenRead()))
            {
                return s.ReadToEnd();
            }
        }

        public byte[] ReadAsBinary()
        {
            using (FileStream s = this.file.Open(FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[this.file.Length];
                s.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public IFileInfo Create(string file_path)
        {
            try
            {
                LocalFileInfo f = new LocalFileInfo(file_path);
                return f;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public LocalFileInfo GetAsLocalFile()
        {
            throw new NotSupportedException();
        }

        public Task<LocalFileInfo> GetAsLocalFileAsync()
        {
            throw new NotSupportedException();
        }
        #endregion

        #region Public properties
        public FileInfo SysFile
        {
            get
            {
                return this.file;
            }
        }
        #endregion

        #region Private fields
        private FileInfo file;
        private string _PathSeparator = new string(Path.DirectorySeparatorChar, 1);
        #endregion
    }
}
