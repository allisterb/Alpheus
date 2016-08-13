using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus
{
    public partial class MySQL : ConfigurationFile<KeyValueSection, KeyValueNode>
    {
        #region Constructors
        public MySQL() : base() { }
        public MySQL(string file_path, bool read_file = true, bool parse_file = true) : base(file_path, read_file, parse_file)
        {
        }
        #endregion
    }

}
