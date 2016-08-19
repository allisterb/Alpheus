using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Alpheus
{
    public partial class SSHDTests
    {
        public SSHD sshd_1;

        public SSHDTests()
        {
            sshd_1 = new SSHD(Path.Combine("SSHD", "sshd_config"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(sshd_1.File.Exists);
        }

    }
}
