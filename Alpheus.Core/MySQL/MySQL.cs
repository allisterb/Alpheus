using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus
{
    public partial class MySQL
    {
        public static Lens<AString> SectionName = new Lens<AString>
        {
            Parser = MySQL.Grammar.SectionName,
            Template = "[{0}]"
        };
    }
}
