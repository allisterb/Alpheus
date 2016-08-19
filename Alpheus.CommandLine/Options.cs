using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace Alpheus.CommandLine
{
    public class Options
    {
        public Options() {}

        [VerbOption("mysql", HelpText = "Open MySQL configuration files.")]
        public Options AuditNuGet { get; set; }

        [VerbOption("sshd", HelpText = "Open SSHD configuration files.")]
        public Options AuditSSHD { get; set; }


        [Option('f', "file", Required = true, HelpText = "Specify the configuration file to open.")]
        public string File { get; set; }

        [Option('x', "xml", Required = false, HelpText = "Print the XML configuration tree for the specified file.")]
        public bool PrintXml { get; set; }

        [Option('e', "evaulate", Required = false, HelpText = "Specify the XPath expression to against the configuration tree.")]
        public string EvaluateXPath { get; set; }

        [Option('P', "print-nodes", Required = false, HelpText = "Print the nodes matched (if any) of XPath expression evaluated against the configuration tree.")]
        public bool PrintNodes { get; set; }

        [Option('n', "non-interact", Required = false, HelpText = "Disable any interctive console output (for redirecting console output to other devices.)")]
        public bool NonInteractive { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
