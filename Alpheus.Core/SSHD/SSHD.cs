using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alpheus.IO;
namespace Alpheus
{
    public partial class SSHD : ConfigurationFile<KeyValues, KeyMultipleValueNode>
    {
        #region Constructors
        public SSHD() : base() { }
        public SSHD(string file_path, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyMultipleValueNode>, string, string> read_file_lambda = null) : base(file_path, string.Empty, read_file, parse_file, read_file_lambda) {}
        public SSHD(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyMultipleValueNode>, string, string> read_file_lambda = null) : base(file, string.Empty, read_file, parse_file, read_file_lambda) { }
        #endregion

        #region Overriden methods
        public override ConfigurationFile<KeyValues, KeyMultipleValueNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<KeyValues, KeyMultipleValueNode>, string, string> read_file_lambda = null)
        {
            return new SSHD(file, read_file, parse_file, read_file_lambda);
        }
        #endregion
    }
}
