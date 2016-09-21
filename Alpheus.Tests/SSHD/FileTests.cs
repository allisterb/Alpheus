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
            Assert.True(!string.IsNullOrEmpty(sshd_1.FileContents));
            SSHD sshd_2 = new SSHD("dynamic file", true, true, (parent, path) =>
                {
                    return parent.FilePath + " dynamic file contents";
                    
                });
            Assert.Equal(sshd_2.FileContents, sshd_2.FilePath + " dynamic file contents");
        }



    }
}
