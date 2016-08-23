using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus
{
    public partial class Nginx : ConfigurationFile<DirectiveSection, DirectiveNode>
    {
        #region Constructors
        public Nginx() : base() { }
        public Nginx(string file_path, bool read_file = true, bool parse_file = true) : base(file_path, read_file, parse_file) {}
        #endregion
    }
}
