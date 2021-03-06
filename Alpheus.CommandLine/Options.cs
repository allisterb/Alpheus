﻿using System;
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
        public Options MySQL { get; set; }

        [VerbOption("sshd", HelpText = "Open SSHD configuration files.")]
        public Options SSHD { get; set; }

        [VerbOption("nginx", HelpText = "Open Nginx configuration files.")]
        public Options Nginx { get; set; }

        [VerbOption("httpd", HelpText = "Open Apache Httpd configuration files.")]
        public Options Httpd { get; set; }

        [VerbOption("netfx", HelpText = "Open .NET Framework configuration files.")]
        public Options NetFx { get; set; }

        [VerbOption("pgsql", HelpText = "Open PostgreSQL configuration files.")]
        public Options PostgreSQL { get; set; }

        [VerbOption("docker", HelpText = "Open Dockerfile.")]
        public Options Dockerfile { get; set; }

        //[VerbOption("docker", HelpText = "Open JSON file.")]
        public Options Json { get; set; }

        [Option('f', "file", Required = false, HelpText = "Specify the configuration file to open.")]
        public string File { get; set; }

        [Option('x', "xml", Required = false, HelpText = "Print the XML configuration tree for the specified file.")]
        public bool PrintXml { get; set; }

        [Option('e', "evaulate", Required = false, HelpText = "Specify the XPath expression to against the configuration tree.")]
        public string EvaluateXPath { get; set; }

        [Option('s', "statistics", Required = false, HelpText = "Print the statistics for the XML configuration tree only.")]
        public bool StatisticsOnly { get; set; }

        [Option('p', "print-nodes", Required = false, HelpText = "Print the nodes matched (if any) of XPath expression evaluated against the configuration tree.")]
        public bool PrintNodes { get; set; }

        [Option('n', "non-interact", Required = false, HelpText = "Disable any interctive console output (for redirecting console output to other devices.)")]
        public bool NonInteractive { get; set; }


        [Option('v', "version", Required = false, HelpText = "Print Alpheus version information.")]
        public bool PrintVersion { get; set; }

        [Option('c', "comments", DefaultValue = false, Required = false, HelpText = "Include comment nodes when printing the XML configuration tree.")]
        public bool IncludeComments { get; set; }

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
