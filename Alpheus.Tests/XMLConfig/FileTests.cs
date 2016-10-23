using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Alpheus
{
    public partial class XMLConfigTests
    {
        public XMLConfig xmlconfig_1;

        public XMLConfigTests()
        {
            xmlconfig_1 = new XMLConfig(Path.Combine("XMLConfig", "Web.config"));
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(xmlconfig_1.File.Exists);
            Assert.True(!string.IsNullOrEmpty(xmlconfig_1.FileContents));
            Assert.True(xmlconfig_1.ParseSucceded);
        }
    }
}
