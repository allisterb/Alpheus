using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Alpheus
{
    public interface IConfiguration
    {
        #region Public properties
        string FilePath { get; }
        FileInfo File { get; }
        string FileContents { get; }
        XDocument XmlConfiguration { get; }
        Exception LastException { get; }
        bool ParseSucceded { get; }
        List<Tuple<string, bool>> IncludeFilesStatus { get; }
        #endregion

        #region Public methods
        bool ReadFile();
        bool XPathEvaluate(string e, out List<string> result, out string message);
        Dictionary<string, Tuple<bool, List<string>, string>> XPathEvaluate(List<string> expressions);
        #endregion
    }
}
