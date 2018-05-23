using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class LogCommand : ICommand
    {
        #region Members
        private ILogging logger;
        #endregion

        public LogCommand(ILogging logger)
        {
            // Storing the Logger
            this.logger = logger;
        }

        string ICommand.Execute(List<string> args, out bool result)
        {
            args = logger.EntriesRequest();
            args.Insert(0, CommandEnum.LogCommand.ToString());
            foreach (string entry in args)
            {
                if (entry.EndsWith("\n")) entry.TrimEnd("\n".ToCharArray());
            }
            result = true;
            return "Log Requested";
        }
    }
}
