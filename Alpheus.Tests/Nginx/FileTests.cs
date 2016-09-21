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
        public Nginx nginx_2;

        public NginxTests()
        {
            nginx_1 = new Nginx(Path.Combine("Nginx", "nginx.conf.1"), true);
            nginx_2 = new Nginx(Path.Combine("Nginx", "mime.types"), true);
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(nginx_1.File.Exists);
        }

        [Fact]
        public void CanParseFile()
        {
            Assert.True(nginx_2.ParseSucceded);
        }

    }
}
