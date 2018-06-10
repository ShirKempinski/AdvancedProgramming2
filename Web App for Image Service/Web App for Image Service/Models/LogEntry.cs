using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web_App_for_Image_Service.Models
{
    public class LogEntry
    {

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Type")]
        public string status { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Message")]
        public string message { get; set; }

        public LogEntry(string status, string message)
        {
            this.status = status;
            this.message = message;
        }

    }

    public class Logs : List<LogEntry> { }
}
