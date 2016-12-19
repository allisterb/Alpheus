using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alpheus.IO;
namespace Alpheus
{
    public partial class MySQL : ConfigurationFile<KeyValueSection, KeyValueNode>
    {
        #region Constructors
        public MySQL() : base() { }
        public MySQL(string file_path, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValueSection, KeyValueNode>, string, string> read_file_lambda = null) : 
            base(file_path, "//include | //includedir", read_file, parse_file, read_file_lambda) {}
        public MySQL(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValueSection, KeyValueNode>, string, string> read_file_lambda = null) : 
            base(file, "//include | //includedir", read_file, parse_file, read_file_lambda) {}
        #endregion

        #region Overriden methods
        public override ConfigurationFile<KeyValueSection, KeyValueNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValueSection, KeyValueNode>, string, string> read_file_lambda = null)
        {
            return new MySQL(file, read_file, parse_file, read_file_lambda);
        }
        #endregion
    }

}
