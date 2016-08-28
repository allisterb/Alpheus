using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Sprache;

namespace Alpheus
{
    public abstract class ConfigurationFile<S, K> : IConfiguration, IConfigurationFactory<S, K> where S : IConfigurationNode where K : IConfigurationNode
    {
        #region Abstract methods and properties
        public abstract ConfigurationTree<S, K> ParseTree(string t);
        public abstract Parser<ConfigurationTree<S, K>> Parser { get; }
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
        public ConfigurationTree<S, K> ConfigurationTree { get; private set; }
        public XDocument XmlConfiguration
        {
            get
            {
                if (this.ConfigurationTree != null && this.ConfigurationTree.Xml != null)
                {
                    return this.ConfigurationTree.Xml;
                }
                else return null;
            }
        }
        public ParseException LastParseException { get; private set; }
        public Exception LastException { get; private set; }
        public bool ParseSucceded { get; private set; } = false;
        public List<Tuple<string, bool, ConfigurationFile<S, K>>> IncludeFiles { get; protected set; }
        public List<Tuple<string, bool>> IncludeFilesStatus
        {
            get
            {
                if (this.IncludeFiles == null) return null;
                else
                {
                    return this.IncludeFiles.Select(f => new Tuple<string, bool>(f.Item1, f.Item2)).ToList();
                }
            }
        }
        #endregion

        #region Constructors
        public ConfigurationFile()
        {

            this.FilePath = "none";
        }

        public ConfigurationFile(string file_path, bool read_file = true, bool parse_file = true) : base()
        {
            this.FilePath = file_path;
            if (read_file)
            {
                if (!this.ReadFile()) return;
            }
            if (read_file && parse_file)
            {
                this.ParseFile();
            }

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
                    this.LastException = e;
                }
                else
                {
                    this.LastException = e;
                }
                return false;
            }
        }

        public virtual void ParseFile()
        {
            if (this.File == null || !this.File.Exists)
            {
                throw new InvalidOperationException(string.Format("The file {0} does not exist.", this.FilePath));
            }
            if (string.IsNullOrEmpty(this.FileContents))
            {
                throw new InvalidOperationException(string.Format("The file contents for {0} have not been read.", this.FilePath));
            }
            try
            {
                this.ConfigurationTree = this.ParseTree(this.FileContents);
                if (this.ConfigurationTree != null && this.ConfigurationTree.Xml != null)
                {
                    this.ParseSucceded = true;
                }
                else
                {
                    this.ParseSucceded = false;
                }
            }
            catch (ParseException pe)
            {
                this.LastParseException = pe;
                this.LastException = pe;
                this.ParseSucceded = false;
            }

        }


        public virtual Dictionary<string, Tuple<bool, List<string>, string>> XPathEvaluate(List<string> expressions)
        {
            if (this.ParseSucceded)
            {
                return this.ConfigurationTree.XPathEvaluate(expressions);
            }
            else
            {
                throw new InvalidOperationException("Parsing configuration failed.");
            }
        }

        public virtual bool XPathEvaluate(string e, out List<string> result, out string message)
        {
            if (this.ParseSucceded)
            {
                return this.ConfigurationTree.XPathEvaluate(e, out result, out message);
            }
            else
            {
                throw new InvalidOperationException("Parsing configuration failed.");
            }
        }
        #endregion

        #region Static methods
        public static string UnescapeSlash(string txt)
        {
            if (string.IsNullOrEmpty(txt)) { return txt; }
            StringBuilder retval = new StringBuilder(txt.Length);
            for (int ix = 0; ix < txt.Length;)
            {
                int jx = txt.IndexOf('\\', ix);
                if (jx < 0 || jx == txt.Length - 1) jx = txt.Length;
                retval.Append(txt, ix, jx - ix);
                if (jx >= txt.Length) break;
                switch (txt[jx + 1])
                {
                  
                    //case 'n': retval.Append('\n'); break;  // Line feed
                    //case 'r': retval.Append('\r'); break;  // Carriage return
                    //case 't': retval.Append('\t'); break;  // Tab
                    case '\\': retval.Append(""); break; // Don't escape
                    default:                                 // Unrecognized, copy as-is
                        retval.Append('\\').Append(txt[jx + 1]); break;
                }
                ix = jx + 2;
            }
            return retval.ToString();
        }
        #endregion
    }
}
