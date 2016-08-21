using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Alpheus
{
    public partial class HttpdTests
    {
        public Httpd httpd_1;

        public HttpdTests()
        {
            httpd_1 = new Httpd(Path.Combine("Httpd", "httpd_conf.1"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(httpd_1.File.Exists);
        }

    }
}
