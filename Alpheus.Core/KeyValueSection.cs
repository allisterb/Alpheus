using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace Alpheus
{
    public class KeyValueSection : Dictionary<AString, AString>
    {
       
        public AString Name { get; set; }

        public KeyValueSection(AString name) : base(10)
        {
            this.Name = name;
        }

        public KeyValueSection(AString name, IEnumerable<KeyValuePair<AString, AString>> keys) : this(name)
        {
            foreach(KeyValuePair<AString, AString> k in keys)
            {
                this.Add(k.Key, k.Value);
            }
        }
    }
}
