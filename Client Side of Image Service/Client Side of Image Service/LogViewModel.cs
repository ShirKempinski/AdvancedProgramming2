using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class LogViewModel: BaseViewModel
    {
        #region members
        private LogModel model;
        public ObservableCollection<LogEntry> logList { get; private set; }
        #endregion

        public LogViewModel()
        {
            model = new LogModel();
            logList = new ObservableCollection<LogEntry>();
            foreach (LogEntry entry in model.logList)
            {
                logList.Add(entry);
            }
            model.logListUpdated += GetNewLog;
        }

        public void GetNewLog(Object sender, LogEntry args)
        {
            logList.Add(args);
        }
    }
}
