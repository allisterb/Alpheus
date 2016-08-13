using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using CL = CommandLine; //Avoid type name conflict with external CommandLine library
using CO = Colorful;

namespace Alpheus.CommandLine
{
    class Program
    {
        public enum ExitCodes
        {
            SUCCESS = 0,
            INVALID_ARGUMENTS,
            NO_PACKAGE_MANAGER,
            ERROR_SCANNING_FOR_PACKAGES,
            ERROR_SEARCHING_OSS_INDEX,
            ERROR_SCANNING_SERVER_VERSION,
            ERROR_SCANNING_SERVER_CONFIGURATION
        }

        static Options ProgramOptions = new Options();

        static ConfigurationFile<KeyValueSection, KeyValueNode> SourceFile { get; set; }

        static CO.Figlet figlet = new CO.Figlet(CO.FigletFont.Load("chunky.flf"));
        static int Main(string[] args)
        {
            #region Handle command line options
            Dictionary<string, object> al_options = new Dictionary<string, object>();
            if (!CL.Parser.Default.ParseArguments(args, ProgramOptions))
            {
                return (int)ExitCodes.INVALID_ARGUMENTS;
            }
            else
            {
                if (!string.IsNullOrEmpty(ProgramOptions.File))
                {
                    if (!File.Exists(ProgramOptions.File))
                    {
                        PrintErrorMessage("Error in parameter: Could not find file {0}.", ProgramOptions.File);
                        return (int)ExitCodes.INVALID_ARGUMENTS;
                    }
                    else
                    {
                        al_options.Add("File", ProgramOptions.File);
                    }
                }

            }
            #endregion

            #region Handle command line verbs
            CL.Parser.Default.ParseArguments(args, ProgramOptions, (verb, options) =>
            {
                try
                {
                    if (verb == "mysql")
                    {
                        SourceFile = new MySQL((string)al_options["File"], true, true);
                        if (SourceFile.LastException != null)
                        {
                            if (SourceFile.LastIOException != null)
                            {
                                PrintErrorMessage(SourceFile.LastIOException);
                            }
                            else if (SourceFile.LastException != null)
                            {
                                PrintErrorMessage(SourceFile.LastException);
                            }

                        }
                    }
                }
                catch (ArgumentException ae)
                {
                    PrintErrorMessage(ae);
                    return;
                }
                catch (Exception e)
                {
                    PrintErrorMessage(e);
                    return;
                }
            });

            if (SourceFile == null)
            {
                Console.WriteLine("No configuration source file specified or error parsing options.");
                return (int)ExitCodes.INVALID_ARGUMENTS;
            }
            #endregion
            PrintBanner();
            PrintXml(SourceFile.ConfigurationTree.Xml);
            return (int)ExitCodes.SUCCESS;
        }

        static void PrintXml(XDocument xml)
        {
            CO.Console.WriteLine(xml.ToString());
        }

        static void PrintBanner()
        {
            //CO.Console.WriteLine(figlet.ToAscii("Alpheus"), Color.PaleGreen);
            Console.WriteLine("v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(), Color.PaleGreen);
        }

        static void PrintMessage(string format)
        {
            Console.Write(format);
        }

        static void PrintMessage(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        static void PrintMessage(ConsoleColor color, string format)
        {
            if (!ProgramOptions.NonInteractive)
            {
                ConsoleColor o = Console.ForegroundColor;
                Console.ForegroundColor = color;
                PrintMessage(format);
                Console.ForegroundColor = o;
            }
            else
            {
                Console.Write(format);
            }
        }

        static void PrintMessage(ConsoleColor color, string format, params object[] args)
        {
            if (!ProgramOptions.NonInteractive)
            {
                ConsoleColor o = Console.ForegroundColor;
                Console.ForegroundColor = color;
                PrintMessage(format, args);
                Console.ForegroundColor = o;
            }
            else
            {
                Console.Write(format, args);
            }
        }

        static void PrintMessageLine(string format)
        {
            Console.WriteLine(format);
        }

        static void PrintMessageLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        static void PrintMessageLine(ConsoleColor color, string format, params object[] args)
        {
            if (!ProgramOptions.NonInteractive)
            {
                ConsoleColor o = Console.ForegroundColor;
                Console.ForegroundColor = color;
                PrintMessageLine(format, args);
                Console.ForegroundColor = o;
            }
            else
            {
                PrintMessageLine(format, args);
            }
        }

        static void PrintErrorMessage(string format, params object[] args)
        {
            PrintMessageLine(ConsoleColor.DarkRed, format, args);
        }

        static void PrintErrorMessage(Exception e)
        {
            PrintMessageLine(ConsoleColor.DarkRed, "Exception: {0}", e.Message);
            PrintMessageLine(ConsoleColor.DarkRed, "Stack trace: {0}", e.StackTrace);

            if (e.InnerException != null)
            {
                PrintMessageLine(ConsoleColor.DarkRed, "Inner exception: {0}", e.InnerException.Message);
                PrintMessageLine(ConsoleColor.DarkRed, "Inner stack trace: {0}", e.InnerException.StackTrace);
            }
        }


    }
}
