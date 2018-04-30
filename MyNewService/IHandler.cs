using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// Interface IHandler has DirectoryClose event, a function that recieves a directory to handle
    /// and a function that invokes the event.
    /// </summary>
    public interface IHandler
    {
        // The Event That Notifies that the Directory is being closed
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        // The Function Recieves the directory to Handle
        void StartHandleDirectory(string dirPath);
        // The Event that will be activated upon new Command
        void OnCommandReceived(object sender, CommandReceivedEventArgs e);
    }
}
