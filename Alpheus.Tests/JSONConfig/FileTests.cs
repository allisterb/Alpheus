using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Alpheus
{
    public partial class JSONConfigTests
    {
        public JSONConfig jsonconfig_1;

        public JSONConfigTests()
        {
            jsonconfig_1 = new JSONConfig(Path.Combine("JSONConfig", "inspect.json"));
        }

        [Fact]
        public void CanReadFile()
        {
            Assert.True(jsonconfig_1.File.Exists);
            Assert.True(!string.IsNullOrEmpty(jsonconfig_1.FileContents));
            Assert.True(jsonconfig_1.ParseSucceded);
        }
    }
}
