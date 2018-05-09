using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class LogEntry
    {
        private string status;
        private string message;

        public LogEntry(string status, string message)
        {
            this.status = status;
            this.message = message;
        }

    }

    public class Logs : List<LogEntry> { }
}
