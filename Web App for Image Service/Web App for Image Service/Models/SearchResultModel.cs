using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Web_App_for_Image_Service.Models
{
    public class SearchResultModel
    {
        public List<LogEntry> logList { get; private set; }

        public SearchResultModel(List<LogEntry> results)
        {
            logList = new List<LogEntry>();
            logList.AddRange(results);
        }
    }
}
