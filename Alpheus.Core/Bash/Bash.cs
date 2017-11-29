using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Alpheus.IO;

namespace Alpheus
{
    public partial class Bash : ConfigurationFile<Commands, CommandNode>
    {
        #region Constructors
        public Bash() : base() { }
        public Bash(string file_path, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<Commands, CommandNode>, string, string> read_file_lambda = null) 
            : base(file_path, string.Empty, read_file, parse_file, read_file_lambda) {}
        public Bash(IFileInfo file, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<Commands, CommandNode>, string, string> read_file_lambda = null) 
            : base(file, string.Empty, new LocalEnvironment(), read_file, parse_file, read_file_lambda) { }
        public Bash(IFileInfo file, AlpheusEnvironment env, bool read_file = true, bool parse_file = true, Func<ConfigurationFile<Commands, CommandNode>, string, string> read_file_lambda = null)
            : base(file, string.Empty, env, read_file, parse_file, read_file_lambda) { }
        #endregion

        #region Overriden methods
        public override ConfigurationFile<Commands, CommandNode> Create(IFileInfo file, bool read_file = true, bool parse_file = true, 
            Func<ConfigurationFile<Commands, CommandNode>, string, string> read_file_lambda = null)
        {
            return new Bash(file, this.AlEnvironment, read_file, parse_file, read_file_lambda);
        }

        public override string PreProcessFile(string file_contents)
        {
            Regex escape_parser_directive_match = new Regex(@"$#\s*\escape=(`|\\)\s*^");
            Regex line_continuation_match = new Regex(this.EscapeChar + @"\r?\n");
            IEnumerable<string> lines = Regex.Split(file_contents, @"\r?\n").Select(s => s.Trim());
            foreach (string l in lines)
            {
                if (!l.StartsWith("#"))
                {
                    break;
                }
                else
                {
                    if (escape_parser_directive_match.IsMatch(l))
                    {
                        this.EscapeChar = escape_parser_directive_match.Match(l).Groups[1].Value;
                        this.AlEnvironment.Debug("Setting escape character to {0}.", this.EscapeChar);
                    }

                }
            }
            file_contents = line_continuation_match.Replace(file_contents, string.Empty);
            return file_contents;
        }
        #endregion

        #region Properties
        public string EscapeChar { get; protected set; } = "\\\\";
        #endregion
    }
}
