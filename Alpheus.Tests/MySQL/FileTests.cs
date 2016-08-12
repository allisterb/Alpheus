using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Alpheus
{
    public partial class MySQLTests
    {
        public MySQL my_1;
        public MySQL my_2;

        public MySQLTests()
        {
            my_1 = new MySQL(Path.Combine("MySQL", "my.cnf"), true);
            my_2 = new MySQL(Path.Combine("MySQL", "my.2.cnf"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(my_1.File.Exists);
        }

    }
}
