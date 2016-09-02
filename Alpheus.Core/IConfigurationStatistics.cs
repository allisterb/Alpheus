using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus
{
    public interface IConfigurationStatistics
    {
        string FullFilePath { get; }
        int? TotalIncludeFiles { get; }
        int? IncludeFilesParsed {get; }
        List<Tuple<string, bool, IConfigurationStatistics>> IncludeFilesStatus { get; }
        int? TotalFileTopLevelNodes { get; }
        int? TotalTopLevelNodes { get; }
        int? FirstLineParsed { get; }
        int? LastLineParsed { get; }
        int? TotalComments { get; }
        int? TotalFileComments { get; }
        //int? TotalSections { get; }
        //int? TotalKeys { get; }
    }
}
