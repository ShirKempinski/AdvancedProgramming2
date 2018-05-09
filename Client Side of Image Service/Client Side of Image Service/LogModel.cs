using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class LogModel
    {
        public event EventHandler<LogEntry> logListUpdated;

        /// <summary>
        /// string status, string message
        /// </summary>
        /// 
        public List<LogEntry> logList { get; }

        public LogModel()
        {
            ClientTCP client = ClientTCP.getInstance();
            if (client.IsConnected())
            {
                client.OnMessageReceived += UpdateLogs;
                client.sendCommand(CommandEnum.LogCommand.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <format> "log status:" + status + "log message:" + message </format>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UpdateLogs(object sender, string args)
        {
            string logCommand = CommandEnum.LogCommand.ToString();
            if (!args.StartsWith(logCommand)) return;
            args = args.TrimStart(logCommand.ToCharArray());
            string[] delimiter = { "log status:", "log message:" };
            string[] statusAndMessage = args.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            LogEntry entry = new LogEntry(statusAndMessage[0], statusAndMessage[1]);
            logList.Add(entry);

            logListUpdated?.Invoke(this, entry);            
            
        }
    }
}
