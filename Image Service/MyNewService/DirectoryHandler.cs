using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ImageService
{
    /// <summary>
    /// class DirectoryHandler. Responsible to watch a directory and make sure that any added
    /// file will have a backup file. the DirctoryHandler also responsible to inform the logger
    /// about any change.
    /// </summary>
    /// <remarks>
    /// Implements the IHandler Interface.
    /// </remarks>
    public class DirectoryHandler : IHandler
    {
        #region Members
        private FileSystemWatcher dirWatcher;
        private IController controller;
        private ILogging logger;
        private String path;
        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        #endregion

        /// <summary>
        /// DirectoryHandler constructor.
        /// </summary>
        /// <param name="controller"> IController interface implementation </param>
        /// <param name="logger"> ILogging interface implementation </param>
        /// <returns> a new object of DirectoryHandler </returns>
        public DirectoryHandler(IController controller, ILogging logger)
        {
            this.controller = controller;
            this.logger = logger;
        }

        /// <summary>
        /// takes the FileSystemEventArgs and if the file is a picture, call the IController to
        /// handle it, and then report to the ILogging.
        /// </summary>
        /// <param name="source"> not used </param>
        /// <param name="e"> the arguments of the new file </param>
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            // get the file's extension 
            string strFileExt = Path.GetExtension(e.FullPath);

            // filter file types
            if (Regex.IsMatch(strFileExt, @"\.jpg|\.png|\.bmp|\.gif|\.jpeg", RegexOptions.IgnoreCase))
            {
                List<String> args = new List<String>();
                args.Add(e.FullPath.TrimEnd(e.Name.ToCharArray()));
                args.Add(e.Name);
                string message = controller.ExecuteCommand(CommandEnum.NewFileCommand, args, out bool result);
                if (result)
                {
                    logger.Log(message, MessageTypeEnum.INFO);
                }
                else
                {
                    logger.Log(message, MessageTypeEnum.FAIL);
                }
            }
        }

        /// <summary>
        /// Create the FileSystemWatcher and add the OnCreated function to it's Created event.
        /// </summary>
        public void CreateWatcher()
        {
            dirWatcher = new FileSystemWatcher();
            dirWatcher.Path = path;
            dirWatcher.Created += new FileSystemEventHandler(OnCreated);
            dirWatcher.IncludeSubdirectories = true;
            dirWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stops the listening to the directory. invokes the DirectoryClose event and informs
        /// the ILogging.
        /// </summary>
        /// <param name="e"> the arguments for the event </param>
        void OnClose(DirectoryCloseEventArgs args)
        {
            try {
                if (path == args.DirectoryPath)
                {
                    path = null;
                    dirWatcher.EnableRaisingEvents = false;
                    DirectoryClose?.Invoke(this, args);
                    logger.Log(args.Message, MessageTypeEnum.INFO);
                }
            } catch (Exception e)
            {
                logger.Log(e.Message, MessageTypeEnum.FAIL);
            }
        }

        /// <summary>
        /// Receives a directory to handle and create a watcher for it.
        /// </summary>
        /// <param name="dirPath"> the directory to handle </param>
        void IHandler.StartHandleDirectory(string dirPath)
        {
            // The Function receives the directory to Handle
            this.path = dirPath;
            CreateWatcher();
        }

        /// <summary>
        /// When a command is being received, this function execute it via the IController, and
        /// then informs the ILogging.
        /// </summary>
        /// <param name="sender"> the object that sent the command </param>
        /// <param name="e"> the command's arguments </param>
        void IHandler.OnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if (e.CommandID == CommandEnum.CloseCommand)
            {
                DirectoryCloseEventArgs args = new DirectoryCloseEventArgs(e.RequestDirPath, "Closing Handler: " + path);
                OnClose(args);
            }
            else
            {
                // The Event that will be activated upon new Command
                string logMsg = controller.ExecuteCommand(e.CommandID, e.Args, out bool result);
                if (result)
                {
                    logger.Log(logMsg, MessageTypeEnum.INFO);
                }
                else
                {
                    logger.Log(logMsg, MessageTypeEnum.FAIL);
                }
            }
        }
    }
}
