using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus.IO
{
    public class LocalDirectoryInfo : IDirectoryInfo
    {
        #region Constructors
        public LocalDirectoryInfo(string dir_path)
        {
            this.directory = new DirectoryInfo(dir_path);
            this.PathSeparator = new string(Path.DirectorySeparatorChar, 1);
        }

        public LocalDirectoryInfo(DirectoryInfo dir) : this(dir.FullName)
        {
            this.directory = dir;
        }
        #endregion

        #region Public properties
        public string PathSeparator { get; private set; } = new string(Path.DirectorySeparatorChar, 1);

        public IEnvironment Environment { get; protected set; }

        public string Name
        {
            get
            {
                return this.directory.Name;
            }
        }

        public string FullName
        {
            get
            {
                return this.directory.FullName;
            }
        }

        public IDirectoryInfo Parent
        {
            get
            {
                return new LocalDirectoryInfo(this.directory.Parent);
            }
        }

        public IDirectoryInfo Root
        {
            get
            {
                return new LocalDirectoryInfo(this.directory.Root);
            }
        }

        public bool Exists
        {
            get
            {
                return this.directory.Exists;
            }
        }
        #endregion

        #region Public methods
        public IDirectoryInfo Create(string file_path)
        {
            try
            {
                LocalDirectoryInfo d = new LocalDirectoryInfo(file_path);
                return d;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IDirectoryInfo[] GetDirectories()
        {
            DirectoryInfo[] dirs = this.directory.GetDirectories();
            return dirs != null ? dirs.Select(d => new LocalDirectoryInfo(d)).ToArray() : null;

        }

        public IDirectoryInfo[] GetDirectories(string searchPattern)
        {
            DirectoryInfo[] dirs = this.directory.GetDirectories(searchPattern);
            return dirs != null ? dirs.Select(d => new LocalDirectoryInfo(d)).ToArray() : null;
           
        }

        public IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            DirectoryInfo[] dirs = this.directory.GetDirectories(searchPattern, searchOption);
            return dirs != null ? dirs.Select(d => new LocalDirectoryInfo(d)).ToArray() : null;
        }

        public IFileInfo[] GetFiles()
        {
            IEnumerable<string> files = Directory.GetFiles(this.directory.FullName, "*.*", SearchOption.AllDirectories).ToArray();
            return files != null ? files.Select(f => new LocalFileInfo(f)).ToArray() : null;
        }

        public IFileInfo[] GetFiles(string searchPattern)
        {
            IEnumerable<string> files = Directory.GetFiles(this.directory.FullName, searchPattern);
            return files != null ? files.Select(f => new LocalFileInfo(f)).ToArray() : null;
        }

        public IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            string[] files = Directory.GetFiles(this.directory.FullName, searchPattern, searchOption);
            return files != null ? files.Select(f => new LocalFileInfo(f)).ToArray() : null;
        }
        #endregion

        #region Methods
        private IEnumerable<string> Search(string root, string searchPattern)
        {
            Queue<string> dirs = new Queue<string>();
            dirs.Enqueue(root);
            while (dirs.Count > 0)
            {
                string dir = dirs.Dequeue();

                // files
                string[] paths = null;
                try
                {
                    paths = Directory.GetFiles(dir, searchPattern);
                }
                catch { } // swallow

                if (paths != null && paths.Length > 0)
                {
                    foreach (string file in paths)
                    {
                        yield return file;
                    }
                }

                // sub-directories
                paths = null;
                try
                {
                    paths = Directory.GetDirectories(dir);
                }
                catch { } // swallow

                if (paths != null && paths.Length > 0)
                {
                    foreach (string subDir in paths)
                    {
                        dirs.Enqueue(subDir);
                    }
                }
            }
        }
        #endregion

        #region Fields
        private DirectoryInfo directory;
        #endregion

    }
}
