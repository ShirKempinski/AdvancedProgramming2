using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class LogEntry
    {
        public string status { get; set; }
        public string message { get; set; }

        public LogEntry(string status, string message)
        {
            this.status = status;
            this.message = message;
        }

    }

    public class Logs : List<LogEntry> { }
}
