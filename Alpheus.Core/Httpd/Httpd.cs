﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Alpheus.IO;
namespace Alpheus
{
    public partial class Httpd : ConfigurationFile<DirectiveSection, DirectiveNode>
    {
        #region Constructors
        public Httpd() : base() { }
        public Httpd(string file_path, bool read_file = true, bool parse_file = true) : base(file_path, "/Httpd/Include/Arg | /Httpd/IncludeOptional/Arg", read_file, parse_file) {}
        public Httpd(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<DirectiveSection, DirectiveNode>, string, string> read_file_lambda = null) 
            : base(file, "/Httpd/Include/Arg | /Httpd/IncludeOptional/Arg", new LocalEnvironment(), read_file, parse_file, read_file_lambda) { }
        public Httpd(IFileInfo file, AlpheusEnvironment env, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<DirectiveSection, DirectiveNode>, string, string> read_file_lambda = null)
     : base(file, "/Httpd/Include/Arg | /Httpd/IncludeOptional/Arg", env, read_file, parse_file, read_file_lambda) { }
        #endregion

        #region Overriden methods
        public override ConfigurationFile<DirectiveSection, DirectiveNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<DirectiveSection, DirectiveNode>, string, string> read_file_lambda = null)
        {
            return new Httpd(file, this.AlEnvironment, read_file, parse_file, read_file_lambda);
        }

        public override string PreProcessFile(string file_contents)
        {
            Regex line_continuation_match = new Regex(@"\\\r?\n");
            return line_continuation_match.Replace(file_contents, string.Empty);
        }
        #endregion
    }
}
