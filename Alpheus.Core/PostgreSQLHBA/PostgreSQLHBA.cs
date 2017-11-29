using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alpheus.IO;
namespace Alpheus
{
    public partial class PostgreSQLHBA : ConfigurationFile<KeyValues, KeyMultipleValueNode>
    {
        #region Constructors
        public PostgreSQLHBA() : base() { }
        public PostgreSQLHBA(string file_path, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyMultipleValueNode>, string, string> read_file_lambda = null) : base(file_path, string.Empty, read_file, parse_file, read_file_lambda) {}
        public PostgreSQLHBA(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyMultipleValueNode>, string, string> read_file_lambda = null) : base(file, string.Empty, read_file, parse_file, read_file_lambda) { }
        #endregion

        #region Overriden methods
        public override ConfigurationFile<KeyValues, KeyMultipleValueNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyMultipleValueNode>, string, string> read_file_lambda = null)
        {
            return new PostgreSQLHBA(file, read_file, parse_file, read_file_lambda);
        }
        #endregion
    }
}
