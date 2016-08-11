using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
namespace Alpheus
{
    public interface IConfigurationFactory<T, S, K> where S : IConfigurationNode where K: IConfigurationNode
    {
        Parser<T> Parser { get; }
        ConfigurationTree<S, K> ConfigurationTree { get; set; }
    }
}
