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
        public MySQL my_1, my_2, my_3, my_4;

        public MySQLTests()
        {
            my_1 = new MySQL(Path.Combine("MySQL", "my.cnf"), true);
            my_2 = new MySQL(Path.Combine("MySQL", "my.2.cnf"), true);
            my_3 = new MySQL(Path.Combine("MySQL", "mysql-multi", "my-large.cnf"), true);
            my_4 = new MySQL(Path.Combine("MySQL", "my.3.cnf"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(my_1.File.Exists);
            Assert.True(my_2.File.Exists);
            Assert.True(my_3.File.Exists);
        }

        [Fact]
        public void CanIncludeFile()
        {
            IConfigurationStatistics s = my_3;
            Assert.True(s.IncludeFilesParsed.HasValue);
            Assert.True(s.IncludeFilesParsed > 0);
        }

    }
}
