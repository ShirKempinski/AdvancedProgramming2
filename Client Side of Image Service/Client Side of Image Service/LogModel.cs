using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client_Side_of_Image_Service
{
    public class LogModel
    {
        public ObservableCollection<LogEntry> logList { get; private set; }

        public LogModel()
        {
            logList = new ObservableCollection<LogEntry>();
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
                Application.Current.Dispatcher.Invoke(delegate
                {
                    foreach (string log in args)
                    {
                        string[] statusAndMessage = log.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        LogEntry entry = new LogEntry(statusAndMessage[0], statusAndMessage[1]);
                        if (!string.IsNullOrEmpty(entry.status)) logList.Insert(0, entry);
                    }
                });
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
