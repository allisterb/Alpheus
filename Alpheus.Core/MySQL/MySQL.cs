using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus
{
    public partial class MySQL : ConfigurationFile, IConfigurationFactory<MySQL, KeyValueSection, KeyValueNode>
    {
        
        public static Lens<AString> SectionName = new Lens<AString>
        {
            Parser = MySQL.Grammar.SectionName,
            Template = "[{0}]"
        };

        public ConfigurationTree<KeyValueSection, KeyValueNode> ConfigurationTree { get; set; }

        #region Public methods
        public ConfigurationTree<KeyValueSection, KeyValueNode> ParseTree(string f)
        {
            throw new NotImplementedException();
            //return Grammar<MySQL, KeyValueSection, KeyValueNode>.p
        }
        #endregion

        #region Constructors
        public MySQL() : base() {}
        public MySQL(string file_path, bool read_file) : base(file_path, read_file) {}
        #endregion
    }
}
