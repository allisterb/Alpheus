using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace Alpheus.CommandLine
{
    public class CLIEnvironment : LocalEnvironment
    {
        #region Constructors
        public CLIEnvironment()
        {
            L = Log.ForContext<CLIEnvironment>();
        }
        #endregion

        #region Overriden methods
        public override void Message(string message_type, string message_format, params object[] message)
        {
            switch (message_type)
            {
                case "INFO":
                case "STATUS":
                case "SUCCESS":
                case "PROGRESS":
                    L.Information(message_format, message);
                    break;
                case "DEBUG":
                    L.Debug(message_format, message);
                    break;
                case "WARNING":
                    L.Warning(message_format, message);
                    break;
                case "ERROR":
                    L.Error(message_format, message);
                    break;
            }
        }
        #endregion

        #region Properties
        ILogger L { get; set; }
        #endregion
    }
}
