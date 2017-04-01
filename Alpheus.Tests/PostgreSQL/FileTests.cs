using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Alpheus
{
    public partial class PostgreSQLTests
    {
        public PostgreSQL pg_1, pg_2, pg_3, pg_4;

        public PostgreSQLTests()
        {
            pg_1 = new PostgreSQL(Path.Combine("PostgreSQL", "postgresql.conf.sample"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(pg_1.File.Exists);
        }

    }
}
