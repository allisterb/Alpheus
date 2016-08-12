using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace Alpheus
{
    public abstract class ConfigurationFile
    {
        #region Abstract methods
        #endregion
        
        #region Public properties
        public string FilePath { get; private set; } 
        public FileInfo File
        {
            get
            {
                return new FileInfo(this.FilePath);
            }

        }
        public string FileContents { get; private set; }
        public IOException LastIOException { get; private set; }
        public Exception LastException { get; private set; }

        #endregion

        #region Constructors
        public ConfigurationFile() 
        {
            this.FilePath = "none";
        }

        public ConfigurationFile(string file_path, bool read_file = true)
        {
            this.FilePath = file_path;
            if (read_file) this.ReadFile();
        }
        #endregion

        #region Public methods
        public virtual bool ReadFile()
        {
            if (!this.File.Exists)
            {
                return false;
            }
            try
            {
                using (StreamReader s = new StreamReader(this.File.OpenRead()))
                {
                    this.FileContents = s.ReadToEnd();
                    return true;
                }
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
                    this.LastIOException = e as IOException;
                }
                else
                {
                    this.LastException = e;
                }
                return false;
            }
        }

      
        #endregion
    }
}
