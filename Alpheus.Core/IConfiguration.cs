using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Alpheus.IO;

namespace Alpheus
{
    public interface IConfiguration : IConfigurationStatistics
    {
        #region Properties
        string FilePath { get; }
        IFileInfo File { get; }
        string FileContents { get; }
        XDocument XmlConfiguration { get; }
        Exception LastException { get; }
        bool ParseSucceded { get; }
        #endregion

        #region Methods
        bool ReadFile();
        bool XPathEvaluate(string e, out List<string> result, out string message, string store_id = null);
        bool XPathEvaluate(string e, out XElement result, out string message, string store_id = null);
        Dictionary<string, Tuple<bool, List<string>, string>> XPathEvaluate(List<string> expressions);
        #endregion
    }
}
