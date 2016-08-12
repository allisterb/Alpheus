using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Xunit;
namespace Alpheus
{
    public partial class MySQLTests
    {
        [Fact]
        public void CanBuildXDocument()
        {

            XDocument x = my_1.ConfigurationTree.Xml;
            Assert.True(x.Declaration.Standalone == "yes");
        }
    }
}
