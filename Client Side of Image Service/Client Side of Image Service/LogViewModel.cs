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
        public ObservableCollection<LogEntry> logList { get { return model.logList; } }
        #endregion

        public LogViewModel()
        {
            model = new LogModel();
        }
    }
}
