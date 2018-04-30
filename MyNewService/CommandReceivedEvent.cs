using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// CommandReceivedEventArgs class. Contains the command type (id), it's arguments and the associated path.
    /// </summary>
    /// <remarks>
    /// Implements the EventArgs interface.
    /// </remarks>
    public class CommandReceivedEventArgs : EventArgs
    {
        #region Members
        public CommandEnum CommandID { get; set; }
        public List<String> Args { get; set; }
        public string RequestDirPath { get; set; }
        #endregion

        /// <summary>
        /// CommandReceivedEventArgs constructor.
        /// </summary>
        /// <param name="id"> command's type </param>
        /// <param name="args"> command's arguments </param>
        /// <param name="path"> associated path </param>
        /// <returns>
        /// A new CommandReceivedEventArgs object
        /// </returns>
        public CommandReceivedEventArgs(CommandEnum id, List<String> args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }
    }
}
