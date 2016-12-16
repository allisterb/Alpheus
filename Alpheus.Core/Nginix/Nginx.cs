using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alpheus.IO;
namespace Alpheus
{
    public partial class Nginx : ConfigurationFile<DirectiveSection, DirectiveNode>
    {
        #region Constructors
        public Nginx() : base() { }
        public Nginx(string file_path, bool read_file = true, bool parse_file = true) : base(file_path, "//include/Arg", read_file, parse_file) {}
        public Nginx(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<DirectiveSection, DirectiveNode>, string, string> read_file_lambda = null) : base(file, "//include/Arg", read_file, parse_file, read_file_lambda) { }
        #endregion

        #region Overriden methods
        public override ConfigurationFile<DirectiveSection, DirectiveNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<DirectiveSection, DirectiveNode>, string, string> read_file_lambda = null)
        {
            return new Nginx(file, read_file, parse_file, read_file_lambda);
        }
        #endregion
    }
}
