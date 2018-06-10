using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Web_App_for_Image_Service.Models
{
    public class LogPageModel
    {
        public List<LogEntry> filteredList { get; private set; }
        private List<LogEntry> logList;
        public LogPageModel()
        {
            logList = new List<LogEntry>();
            ClientTCP client = ClientTCP.getInstance();
            if (client.isConnected)
            {
                ClientTCP.OnMessageReceived += UpdateLogs;
                client.sendCommand(CommandEnum.LogCommand.ToString());
            }
            logList.Add(new LogEntry("INFO", "bambam"));
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
                    foreach (string log in args)
                    {
                        string[] statusAndMessage = log.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        LogEntry entry = new LogEntry(statusAndMessage[0], statusAndMessage[1]);
                        if (!string.IsNullOrEmpty(entry.status)) logList.Insert(0, entry);
                    }
                
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public List<LogEntry> SearchLogs(string type)
        {
            if (string.IsNullOrEmpty(type)) return logList; //All entries
            filteredList = logList.Where(entry => entry.status == type).ToList();
        }
    }
}
