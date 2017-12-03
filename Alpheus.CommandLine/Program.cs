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
using Colorful;
using Console = Colorful.Console;
using Serilog;

namespace Alpheus.CommandLine
{
    class Program
    {
        public enum ExitCodes
        {
            SUCCESS = 0,
            INVALID_ARGUMENTS,
            INVALID_XPATH,
            RUNTIME_ERROR
        }

        static ILogger L;

        static string AlpheusDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        static ConsoleColor ForegroundColor = System.Console.ForegroundColor;

        static ConsoleColor BackgroundColor = System.Console.BackgroundColor;

        static Color CoForegroundColor = Console.ForegroundColor;

        static Color CoBackgroundColor = Console.BackgroundColor;

        static Options ProgramOptions = new Options();

        static IConfiguration Source { get; set; }

        static Figlet figlet = new Figlet(FigletFont.Load("chunky.flf"));

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();
            L = Log.ForContext<Program>();

            #region Handle command line options
            Dictionary<string, object> al_options = new Dictionary<string, object>();
            if (!CL.Parser.Default.ParseArguments(args, ProgramOptions))
            {
                Exit(ExitCodes.INVALID_ARGUMENTS);
            }
            else
            {
                if (ProgramOptions.PrintVersion)
                {
                    PrintBanner();
                    Exit(ExitCodes.SUCCESS);
                }

                if (!string.IsNullOrEmpty(ProgramOptions.File))
                {
                    if (!File.Exists(ProgramOptions.File))
                    {
                        PrintErrorMessage("Error in parameter: Could not find file {0}.", ProgramOptions.File);
                        Exit(ExitCodes.INVALID_ARGUMENTS);
                    }
                    else
                    {
                        al_options.Add("File", ProgramOptions.File);
                    }
                }
                else if (string.IsNullOrEmpty(ProgramOptions.AnalyzeXPath))
                {
                    PrintErrorMessage("You must specify a configuration source with the -f/--file option.");
                    Exit(ExitCodes.INVALID_ARGUMENTS);
                }
            }
            #endregion

            #region Handle command line verbs
            try
            {
                CL.Parser.Default.ParseArguments(args, ProgramOptions, (verb, options) =>
                {
                    if (verb == "mysql")
                    {
                        Source = new MySQL((string)al_options["File"], true, true);
                    }
                    else if (verb == "sshd")
                    {
                        Source = new SSHD((string)al_options["File"], true, true);
                    }
                    else if (verb == "nginx")
                    {
                        Source = new Nginx((string)al_options["File"], true, true);
                    }
                    else if (verb == "httpd")
                    {
                        Source = new Httpd((string)al_options["File"], true, true);
                    }
                    else if (verb == "netfx")
                    {
                        Source = new XMLConfig((string)al_options["File"], true, true);
                    }
                    else if (verb == "pgsql")
                    {
                        Source = new PostgreSQL((string)al_options["File"], true, true);
                    }
                    else if (verb == "docker")
                    {
                        Source = new Dockerfile((string)al_options["File"], true, true);
                    }
                    else if (verb == "json")
                    {
                        Source = new JSONConfig((string)al_options["File"], true, true);
                    }
                    if (Source.LastException != null)
                    {
                        throw Source.LastException;
                    }

                });
            }
            catch (ArgumentException ae)
            {
                PrintErrorMessage(ae);
            }
            catch (Exception e)
            {
                PrintErrorMessage(e);
            }
            if (Source == null && string.IsNullOrEmpty(ProgramOptions.AnalyzeXPath))
            {
                Console.WriteLine("No configuration source specified.");
                Exit(ExitCodes.INVALID_ARGUMENTS);
            }            
            #endregion

            PrintMessageLine("Using configuration file: {0}, size: {1} bytes, last modified at: {2} UTC.", Source.File.Name, Source.File.Length, Source.File.LastWriteTimeUtc);
            if (Source.IncludeFilesStatus != null)
            {
                PrintMessageLine("Successfully processed {0} include files out of {1} total.", Source.IncludeFilesStatus.Count(f => f.Item2), Source.IncludeFilesStatus.Count());  
            }

            if (ProgramOptions.StatisticsOnly)
            {
                if (Source.IncludeFilesStatus != null)
                {
                    foreach (Tuple<string, bool, IConfigurationStatistics> status in Source.IncludeFilesStatus)
                    {
                        if (status.Item2)
                        {
                            IConfigurationStatistics include_statistics = status.Item3;
                            PrintMessageLine("Included {0}. File path: {1}. First line parsed {2}. Last line parsed: {3}. Parsed {4} top-level configuration nodes. Parsed {5} comments.", status.Item1, include_statistics.FullFilePath, include_statistics.FirstLineParsed, include_statistics.LastLineParsed, include_statistics.TotalFileTopLevelNodes, include_statistics.TotalFileComments);
                        }
                        else PrintErrorMessage("Failed to include {0}.", status.Item1);
                    }
                }
                PrintMessageLine("First line parsed: {0}. Last line parsed: {1}. Parsed {2} total top-level configuration nodes. Parsed {3} total comments.", Source.FirstLineParsed, Source.LastLineParsed, Source.TotalTopLevelNodes, Source.TotalComments);

                Exit(ExitCodes.SUCCESS);
            }
            else if (ProgramOptions.PrintXml)
            {
                PrintXml(Source.XmlConfiguration);
                Exit(ExitCodes.SUCCESS);
            }
            else if (!string.IsNullOrEmpty(ProgramOptions.EvaluateXPath))
            {
                bool r = Source.XPathEvaluate(ProgramOptions.EvaluateXPath, out List<string> result, out string message);
                if (r)
                {
                    PrintMessageLine("{0}", r);
                }
                else if (!r && message == string.Empty)
                {
                    PrintMessageLine("{0}", r);
                }
                else
                {
                    PrintMessageLine(Color.Red, "{0}", message);
                }
                if (r && ProgramOptions.PrintNodes && result != null)
                {
                    foreach (string x in result)
                    {
                        PrintMessageLine("{0}", x);
                    }

                }
                Exit(ExitCodes.SUCCESS);
            }
            else
            {
                PrintErrorMessage("Select an operation to perform on the file or use option -h for help");
                Exit(ExitCodes.INVALID_ARGUMENTS);
            }
            
        }

        static void PrintXml(XDocument xml)
        {
            StyleSheet styleSheet = new StyleSheet(Console.ForegroundColor);
            styleSheet.AddStyle($"^<{xml.Root.Name.LocalName}>", Color.White);
            styleSheet.AddStyle("\\<[\\w|-]+\\s", Color.Green);
            styleSheet.AddStyle("\\</[\\w|-]+\\>\\s", Color.Green);
            styleSheet.AddStyle("Position\\=\\\"\\d+\\\"", Color.AntiqueWhite);
            styleSheet.AddStyle("Line\\=\\\"\\d+\\\"", Color.AntiqueWhite);
            styleSheet.AddStyle("Column\\=\\\"\\d+\\\"", Color.AntiqueWhite);
            styleSheet.AddStyle("Length\\=\\\"\\d+\\\"", Color.AntiqueWhite);
            styleSheet.AddStyle("File\\=\\\"\\w\\S+\\\"", Color.Aqua);
            Console.WriteStyled(xml.ToString(), styleSheet);
        }

        static void PrintBanner()
        {
#if NETCOREAPP2_0
            string target = ".NET Core 2.0";
#else
            string target = ".NET Framework 4.5";
#endif
            string version = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} for {target}";
            Console.WriteLine(figlet.ToAscii("Alpheus "));
            Console.WriteLine(version);
        }

        static void PrintMessage(string format)
        {
            Console.Write(format);
        }

        static void PrintMessage(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        static void PrintMessage(Color color, string format)
        {
            if (!ProgramOptions.NonInteractive)
            {
                Color o = Console.ForegroundColor;
                Console.ForegroundColor = color;
                PrintMessage(format);
                Console.ForegroundColor = o;
            }
            else
            {
                Console.Write(format);
            }
        }

        static void PrintMessage(Color color, string format, params object[] args)
        {
            if (!ProgramOptions.NonInteractive)
            {
                Color o = Console.ForegroundColor;
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

        static void PrintMessageLine(Color color, string format, params object[] args)
        {
            if (!ProgramOptions.NonInteractive)
            {
                Color o = Console.ForegroundColor;
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
            PrintMessageLine(Color.DarkRed, format, args);
        }

        static void PrintErrorMessage(Exception e)
        {
            PrintMessageLine(Color.DarkRed, "Exception: {0}", e.Message);
            PrintMessageLine(Color.DarkRed, "Stack trace: {0}", e.StackTrace);

            if (e.InnerException != null)
            {
                PrintMessageLine(Color.DarkRed, "Inner exception: {0}", e.InnerException.Message);
                PrintMessageLine(Color.DarkRed, "Inner stack trace: {0}", e.InnerException.StackTrace);
            }
        }

        static void Exit(ExitCodes result)
        {
            Log.CloseAndFlush();
            System.Console.ForegroundColor = ForegroundColor;
            Console.ForegroundColor = CoForegroundColor;
            Console.BackgroundColor = CoBackgroundColor;
            System.Console.BackgroundColor = BackgroundColor;
            Environment.Exit((int)result);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            PrintErrorMessage("Unhandled runtime exception occurred. Alpheus will terminate now.");
            PrintErrorMessage((Exception)e.ExceptionObject);
            System.Console.ForegroundColor = ForegroundColor;
            Console.ForegroundColor = CoForegroundColor;
            Console.BackgroundColor = CoBackgroundColor;
            System.Console.BackgroundColor = BackgroundColor;
            Environment.Exit((int) ExitCodes.RUNTIME_ERROR);
        }

    }
}
