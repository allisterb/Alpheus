using System;
using System.Reflection;
using System.Collections;
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
        }

        static Options ProgramOptions = new Options();

        static ConfigurationFile<KeyValueSection, KeyValueNode> Source { get; set; }

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
                        Source = new MySQL((string)al_options["File"], true, true);
                        if (Source.LastException != null)
                        {
                            if (Source.LastIOException != null)
                            {
                                PrintErrorMessage(Source.LastIOException);
                            }
                            else if (Source.LastException != null)
                            {
                                PrintErrorMessage(Source.LastException);
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

            if (Source == null)
            {
                Console.WriteLine("No configuration source specified or error loading file.");
                return (int)ExitCodes.INVALID_ARGUMENTS;
            }
            #endregion
            PrintBanner();
            PrintMessageLine("Using configuration file: {0}, size: {1} bytes, last modified at: {2} UTC.", Source.File.Name, Source.File.Length, Source.File.LastWriteTimeUtc);
            if (ProgramOptions.PrintXml)
            {
                PrintXml(Source.ConfigurationTree.Xml);
            }
            if (!string.IsNullOrEmpty(ProgramOptions.EvaluateXPath))
            {
                IEnumerable result;
                string message;
                bool r = Source.ConfigurationTree.XPathEvaluate(ProgramOptions.EvaluateXPath, out result, out message);
                if (r)
                {
                    PrintMessageLine(ConsoleColor.Green, "{0}", r);
                }
                else if (!r && message == string.Empty)
                {
                    PrintMessageLine(ConsoleColor.Gray, "{0}", r);
                }
                else
                {
                    PrintMessageLine(ConsoleColor.Red, "{0}", message);
                }
                if (r && ProgramOptions.PrintNodes)
                {
                    foreach (XObject x in result)
                    {
                        PrintMessage("{0}", x);
                    }
                    
                }
                return (int)ExitCodes.SUCCESS;
            }
            return (int)ExitCodes.SUCCESS;
        }

        static void PrintXml(XDocument xml)
        {
            CO.Console.WriteLine(xml.ToString());
        }

        static void PrintBanner()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                CO.Console.WriteLine(figlet.ToAscii("Alpheus"), Color.PaleGreen);
                CO.Console.WriteLine("v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(), Color.PaleGreen);
            }
            else
            {
                PrintMessageLine(ConsoleColor.Green, "Alpheus");
                PrintMessageLine(ConsoleColor.Green, "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(), Color.PaleGreen);
            }
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
