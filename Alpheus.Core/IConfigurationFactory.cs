using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
namespace Alpheus
{
    public interface IConfigurationFactory<S, K> where S : IConfigurationNode where K: IConfigurationNode
    {
        Parser<ConfigurationTree<S, K>> Parser { get; }
        ConfigurationTree<S, K> ConfigurationTree { get; }
        FileInfo File { get; }
        ConfigurationTree<S, K> ParseTree(string d);
        ParseException LastParseException { get; }
        bool ParseSucceded { get; }
    }
}
