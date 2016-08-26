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
        public Httpd httpd_2;
        public Httpd httpd_3;

        public HttpdTests()
        {
            httpd_1 = new Httpd(Path.Combine("Httpd", "httpd.conf.1"), true);
            httpd_2 = new Httpd(Path.Combine("Httpd", "httpd.conf.2"), true);
            httpd_3 = new Httpd(Path.Combine("Httpd", "httpd.conf.3"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(httpd_1.File.Exists);
            Assert.True(httpd_2.File.Exists);
            Assert.True(httpd_3.File.Exists);
        }

    }
}
