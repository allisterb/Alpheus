using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Alpheus
{
    public partial class NginxTests
    {
        public Nginx nginx_1;

        public NginxTests()
        {
            //nginx_1 = new Nginx(Path.Combine("Nginx", "nginx.conf.1"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            //Assert.True(nginx_1.File.Exists);
        }

    }
}
