using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class LogEntriesEventArgs : EventArgs
    {
        #region Members
        public List<String> Args { get; set; }
        #endregion

        public LogEntriesEventArgs()
        {
            Args = new List<String>();
        }
    }
}
