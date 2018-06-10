using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Web_App_for_Image_Service.Models
{
    public class LogPageModel
    {
        public Mutex logListMutex;
        public List<LogEntry> filteredList { get; private set; }
        private List<LogEntry> logList;
        public LogPageModel()
        {
            logListMutex = new Mutex();
            logList = new List<LogEntry>();
            filteredList = logList;
            ClientTCP client = ClientTCP.getInstance();
            if (client.isConnected)
            {
                ClientTCP.OnMessageReceived += UpdateLogs;
                client.sendCommand(CommandEnum.LogCommand.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <format> "log status:" + status + "log message:" + message </format>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UpdateLogs(object sender, List<string> args)
        {
            string logCommand = CommandEnum.LogCommand.ToString();
            if (args[0] != logCommand) return;
            args.Remove(logCommand);
            string[] delimiter = { "Message:", "Status:" };
            try
            {
                logListMutex.WaitOne();
                foreach (string log in args)
                {
                    string[] statusAndMessage = log.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    LogEntry entry = new LogEntry(statusAndMessage[0], statusAndMessage[1]);
                    if (!string.IsNullOrEmpty(entry.status)) logList.Insert(0, entry);
                }
                logListMutex.ReleaseMutex();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void SearchLogs(string type)
        {
            logListMutex.WaitOne();
            if (string.IsNullOrEmpty(type)) filteredList = logList; //All entries
            filteredList = logList.Where(entry => entry.status.StartsWith(type)).ToList();
            logListMutex.ReleaseMutex();
        }
    }
}
