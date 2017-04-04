using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

using Sprache;
using Alpheus.IO;

namespace Alpheus
{
    public abstract class ConfigurationFile<S, K> : IConfiguration, IConfigurationStatistics, IConfigurationFactory<S, K> where S : IConfigurationNode where K : IConfigurationNode
    {
        #region Constructors
        public ConfigurationFile()
        {
            this.FilePath = "none";
        }

        public ConfigurationFile(IFileInfo file, string include_file_xpath, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<S, K>, string, string> read_file_lambda = null)
        {
            this.IncludeFileXPath = include_file_xpath;
            this._File = file;
            this.FilePath = this._File.FullName;
            this.ReadFile_ = read_file_lambda;
            if (read_file)
            {
                if (!this.ReadFile()) return;
            }
            if (read_file && parse_file)
            {
                this.ParseFile();
            }
        }

        public ConfigurationFile(string file_path, string include_file_xpath, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<S, K>, string, string> read_file_lambda = null) : this(new LocalFileInfo(file_path), include_file_xpath, read_file, parse_file, read_file_lambda) {}
        #endregion

        #region Abstract methods and properties
        public abstract Parser<ConfigurationTree<S, K>> Parser { get; }
        public abstract ConfigurationFile<S, K> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<S, K>, string, string> read_file_lambda = null);
        #endregion

        #region Public properties
        public string FilePath { get; private set; }

        public string IncludeFileXPath { get; protected set; }

        public IFileInfo File
        {
            get
            {
                if (ReadFile_ != null)
                {
                    return null;
                }
                else
                {
                    return this._File;
                }
            }

        }
        public string FileContents { get; protected set; }
        public Func<ConfigurationFile<S, K>, string, string> ReadFile_ { get; private set; }
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
        public List<Tuple<string, Exception, ParseException>> IncludeFilesExceptions { get; protected set; } = new List<Tuple<string, Exception, ParseException>>();
        public string FullFilePath
        {
            get
            {
                return this.File.FullName;
            }
        }
        public List<Tuple<string, bool, IConfigurationStatistics>> IncludeFilesStatus
        {
            get
            {
                if (this.IncludeFiles == null) return null;
                else
                {
                    return this.IncludeFiles.Select(f => new Tuple<string, bool, IConfigurationStatistics>(f.Item1, f.Item2, f.Item3 as IConfigurationStatistics)).ToList();
                }
            }
        }

        public virtual int? TotalIncludeFiles
        { 
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    return this.IncludeFiles?.Count();
                }
            }
        }

        public virtual int? IncludeFilesParsed
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    return this.IncludeFiles?.Count(i => i.Item2);
                }
            }
        }

        public virtual int? TotalFileTopLevelNodes
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    IEnumerable<XElement> top =
                        from e in this.XmlConfiguration.Root.Elements()
                        where e.Attribute("File").Value == this.File.Name
                        select e;
                    return top.Count();
                }
            }
        }

        public virtual int? TotalTopLevelNodes
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    return this.XmlConfiguration.Root.Elements().Count();
                }
            }
        }

        public virtual int? FirstLineParsed
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    IEnumerable<int> lines =
                        from r in this.XmlConfiguration.Root.Descendants()
                        where r.Attribute("File").Value == this.File.Name && r.Attribute("Line") != null
                        select Int32.Parse(r.Attribute("Line").Value);

                    return lines.Count() == 0 ? 0 : lines.Min();
                }
            }
        }

        public virtual int? LastLineParsed
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    IEnumerable<int> lines =
                        from r in this.XmlConfiguration.Root.Descendants()
                        where r.Attribute("File").Value == this.File.Name && r.Attribute("Line") != null
                        select Int32.Parse(r.Attribute("Line").Value);

                    return lines.Count() == 0 ? 0 : lines.Max();
                }
            }
        }

        public virtual int? TotalFileComments
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    IEnumerable<XElement> comments =
                        from r in this.XmlConfiguration.Root.Descendants()
                        where r.Attribute("File").Value == this.File.Name && r.Name.LocalName.Contains("Comment")
                        select r;
                    return comments.Count();
                }
            }
        }

        public virtual int? TotalComments
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    IEnumerable<XElement> comments =
                        from r in this.XmlConfiguration.Root.Descendants()
                        where r.Name.LocalName.Contains("Comment")
                        select r;
                    return comments.Count();
                }
            }
        }

        public virtual int? TotalKeys
        {
            get
            {
                if (!this.ParseSucceded)
                {
                    return null;
                }
                else
                {
                    IEnumerable<XElement> k =
                        from r in this.XmlConfiguration.Root.Descendants()
                        where r.Attribute("File").Value == this.File.Name && (r.DescendantsAndSelf("Arg") != null || r.DescendantsAndSelf("Value") != null)
                        select r;
                    return k.Count();
                }
            }
        }
        #endregion 

        #region Public methods
        public virtual bool ReadFile()
        {
            if (this.ReadFile_ != null)
            {
                string fc =  this.ReadFile_.Invoke(this, this.FilePath);
                if (!string.IsNullOrEmpty(fc))
                {
                    this.FileContents = fc;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!this.File.Exists)
                {
                    return false;
                }
                try
                {
                    this.FileContents = this.File.ReadAsText();
                    return true;
                    
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

        }

        public virtual void ParseFile()
        {
            if (this.ReadFile_ == null && (this.File == null || !this.File.Exists))
            {
                throw new InvalidOperationException(string.Format("The local file {0} does not exist and no method to read file contents was specified.", this.FilePath));
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
            catch (Exception e)
            {
                this.LastException = e;
                this.ParseSucceded = false;
            }
        }

        public virtual ConfigurationTree<S, K> ParseTree(string f)
        {
            ConfigurationTree<S, K> tree = this.Parser.Parse(f);
            IEnumerable<XElement> ce = tree.Xml.Root.Descendants();
            foreach (XElement element in ce)
            {
                if (element.Attribute("File") == null) element.Add(new XAttribute("File", this.File.Name));
            }
            if (!string.IsNullOrEmpty(this.IncludeFileXPath))
            {
                this.ProcessIncludeFiles(tree, this.IncludeFileXPath);
            }
            return tree;
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

        public virtual bool XPathEvaluate(string e, out XElement result, out string message)
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

        public virtual void ProcessIncludeFiles(ConfigurationTree<S, K> tree, string include_xpath)
        {
            object r = tree.Xml.XPathEvaluate(include_xpath);
            if (r is IEnumerable)
            {
                IEnumerable results = r as IEnumerable;
                this.IncludeFiles = new List<Tuple<string, bool, ConfigurationFile<S, K>>>();
                foreach (XObject o in results)
                {
                    if (o is XElement)
                    {
                        XElement e = o as XElement;
                        string fn = e.Value;
                        if (string.IsNullOrEmpty(fn)) continue;
                        fn = fn.Replace("/", this.File.PathSeparator);
                        IDirectoryInfo root = null;
                        if (fn.StartsWith(this.File.PathSeparator))
                        {
                            root = this.File.Directory.Root;
                        }
                        else
                        {
                            root = this.File.Directory;
                        }
                        IFileInfo include_file = null;
                        try
                        {
                            include_file = this.File.Create(fn); //try as absolute file path
                        }
                        catch (Exception) {}
                        if (include_file != null && include_file.Exists) 
                        {
                            ConfigurationFile<S, K> conf = this.Create(include_file);
                            if (conf.ParseSucceded)
                            {
                                IEnumerable<XElement> child_elements = conf.XmlConfiguration.Root.Elements();
                                tree.Xml.Root.Add(child_elements);
                            }
                            else
                            {
                                this.IncludeFilesExceptions.Add(new Tuple<string, Exception, ParseException>(fn, conf.LastException, conf.LastParseException));
                            }
                            this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<S, K>>
                                (fn, conf.ParseSucceded, conf.ParseSucceded ? conf : null));
                        }
                        else //try as file path relative to current directory
                        {
                            include_file = null;
                            try
                            {
                                include_file = this.File.Create(this.File.DirectoryName + this.File.PathSeparator + fn);
                            }
                            catch (Exception) {}
                            if (include_file != null && include_file.Exists)
                            {
                                ConfigurationFile<S, K> conf = this.Create(include_file);
                                if (conf.ParseSucceded)
                                {
                                    IEnumerable<XElement> child_elements = conf.XmlConfiguration.Root.Elements();
                                    tree.Xml.Root.Add(child_elements);
                                }
                                else
                                {
                                    this.IncludeFilesExceptions.Add(new Tuple<string, Exception, ParseException>(fn, conf.LastException, conf.LastParseException));
                                }
                                this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<S, K>>
                                    (fn, conf.ParseSucceded, conf.ParseSucceded ? conf : null));
                            }
                            else //try as absolute dir path
                            {
                                string dn = fn;
                                IDirectoryInfo dir = null;
                                try
                                {
                                    dir = this.File.Directory.Root.Create(dn);
                                }
                                catch (Exception) {}
                                List<IFileInfo> files = null;
                                if (dir.Exists)
                                {
                                    try
                                    {
                                        files = dir.GetFiles().ToList();
                                    }
                                    catch (Exception) {}
                                }
                                if (files != null && files.Count() > 0)
                                {
                                    foreach (IFileInfo file in files)
                                    {
                                        include_file = file;
                                        try
                                        {
                                            if (include_file.Exists)
                                            {
                                                ConfigurationFile<S, K> conf = this.Create(include_file);
                                                if (conf.ParseSucceded)
                                                {
                                                    IEnumerable<XElement> child_elements = conf.XmlConfiguration.Root.Elements();
                                                    tree.Xml.Root.Add(child_elements);
                                                }
                                                else
                                                {
                                                    this.IncludeFilesExceptions.Add(new Tuple<string, Exception, ParseException>(fn, conf.LastException, conf.LastParseException));
                                                }
                                                this.IncludeFiles.Add(new Tuple<string, bool,
                                                    ConfigurationFile<S, K>>(include_file.Name, conf.ParseSucceded, conf.ParseSucceded ? conf : null));
                                            }
                                            else
                                            {
                                                this.IncludeFiles.Add(new Tuple<string, bool,
                                                    ConfigurationFile<S, K>>(include_file.Name, false, null));
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            this.IncludeFiles.Add(new Tuple<string, bool,
                                                    ConfigurationFile<S, K>>(include_file.Name, false, null));
                                        }
                                    }
                                }                                    
                                else //try as dir name relative to current directory
                                {
                                    IDirectoryInfo[] dirs = null;
                                    List<IFileInfo> dirs_files = new List<IFileInfo>();
                                    try
                                    {
                                        dirs = root.GetDirectories(dn);
                                    }
                                    catch (Exception) {}                                        
                                    if (dirs != null && dirs.Count() > 0)
                                    {
                                        foreach (IDirectoryInfo d in dirs)
                                        {
                                            IFileInfo[] _files = d.GetFiles();
                                            if (_files != null && _files.Count() > 0)
                                            {
                                                dirs_files.AddRange(_files);
                                            }
                                        }
                                    }
                                    if (dirs_files.Count > 0)
                                    {
                                        if (files == null)
                                        {
                                            files = new List<IFileInfo>(dirs_files.Count);
                                        }
                                        files.AddRange(dirs_files);
                                        foreach (IFileInfo file in files)
                                        {
                                            include_file = file;
                                            try
                                            {
                                                if (include_file.Exists)
                                                {
                                                    ConfigurationFile<S, K> conf = this.Create(include_file);
                                                    if (conf.ParseSucceded)
                                                    {
                                                        IEnumerable<XElement> child_elements = conf.XmlConfiguration.Root.Elements();
                                                        tree.Xml.Root.Add(child_elements);
                                                    }
                                                    else
                                                    {
                                                        this.IncludeFilesExceptions.Add(new Tuple<string, Exception, ParseException>(fn, conf.LastException, conf.LastParseException));
                                                    }
                                                    this.IncludeFiles.Add(new Tuple<string, bool,
                                                        ConfigurationFile<S, K>>(include_file.Name, conf.ParseSucceded, conf.ParseSucceded ? conf : null));
                                                }
                                                else
                                                {
                                                    this.IncludeFiles.Add(new Tuple<string, bool,
                                                        ConfigurationFile<S, K>>(include_file.Name, false, null));
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                this.IncludeFiles.Add(new Tuple<string, bool,
                                                        ConfigurationFile<S, K>>(include_file.Name, false, null));
                                            }
                                        }
                                    }
                                    else
                                    {                                                                      
                                        this.IncludeFiles.Add(new Tuple<string, bool, ConfigurationFile<S, K>>
                                        (fn, false, null));   
                                    }   
                                }
                            }
                        } 
                    }
                }

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

        #region Private and protected members
        protected IFileInfo _File;
        #endregion
    }
}
