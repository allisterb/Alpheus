using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alpheus.IO;
namespace Alpheus
{
    public partial class SSHD : ConfigurationFile<KeyValues, KeyValueNode>
    {
        #region Constructors
        public SSHD() : base() { }
        public SSHD(string file_path, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyValueNode>, string, string> read_file_lambda = null) : base(file_path, read_file, parse_file, read_file_lambda) {}
        public SSHD(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyValueNode>, string, string> read_file_lambda = null) : base(file, read_file, parse_file, read_file_lambda) { }
        #endregion
    }
}
