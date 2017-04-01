using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alpheus.IO;
namespace Alpheus
{
    public partial class PostgreSQL : ConfigurationFile<KeyValues, KeyValueNode>
    {
        #region Constructors
        public PostgreSQL() : base() { }
        public PostgreSQL(string file_path, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyValueNode>, string, string> read_file_lambda = null) : 
            base(file_path, "//include | //include_dir", read_file, parse_file, read_file_lambda) {}
        public PostgreSQL(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyValueNode>, string, string> read_file_lambda = null) : 
            base(file, "//include | //include_dir", read_file, parse_file, read_file_lambda) {}
        #endregion

        #region Overriden methods
        public override ConfigurationFile<KeyValues, KeyValueNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyValueNode>, string, string> read_file_lambda = null)
        {
            return new PostgreSQL(file, read_file, parse_file, read_file_lambda);
        }
        #endregion
    }

}
