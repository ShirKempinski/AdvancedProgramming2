using System;
using System.Collections.Generic;
using System.Configuration;

namespace ImageService
{
    /// <summary>
    /// The Server class, responsible for communication with the clients.
    /// Sets the Handlers.
    /// </summary>
    /// <remarks>
    /// Receives commands from the client and pass them to the DirectoryHandlers.
    /// </remarks>
    class Server
    {
        #region Members
        private ILogging logger;
        private IController controller;

        // The event that notifies about a new Command being received
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;
        #endregion

        /// <summary>
        /// Server constructor.
        /// </summary>
        /// <param name="logger">ILogging interface implementation</param>
        /// <param name="controller">IController interface implementation</param>
        /// <returns>
        /// A new Server object
        /// </returns>
        public Server(ILogging logger, IController controller)
        {
            this.logger = logger;
            this.controller = controller;
        }

        /// <summary>
        /// Starts listening to changes in the direcotries specified on App.config.
        /// </summary>
        /// <remarks>
        /// Creates an IHandler per target directory.
        /// </remarks>
        public void Start()
        {
            string handlers = ConfigurationManager.AppSettings["Handler"];
            string[] directories = handlers.Split(';');
            foreach (string directory in directories)
            {
                CreateHandler(directory);
            }
        }

        /// <summary>
        /// Server constructor.
        /// </summary>
        /// <param name="logger">ILogging interface implementation</param>
        /// <param name="controller">ILogging interface implementation</param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// </remarks>
        private void CreateHandler(string directory)
        {
            IHandler h = new DirectoryHandler(controller, logger);
            h.StartHandleDirectory(directory);
            CommandReceived += h.OnCommandReceived;
            h.DirectoryClose += OnCloseServer;
        }

        /// <summary>
        /// Invokes the CommandReceived event and passes the appropriate args to the subscribed Objects.
        /// </summary>
        /// <param name="id">CommandEnum the code for the specific command</param>
        /// <param name="args">List<String> the args used by the Command</param>
        /// <param name="path">String, the target directory/file path</param>
        public void SendCommand(CommandEnum id, List<String> args, string path)
        {
            CommandReceivedEventArgs cArgs = new CommandReceivedEventArgs(id, args, path);
            this.CommandReceived?.Invoke(this, cArgs);
        }

        /// <summary>
        /// Used to inform the server it must remove the directory from it's CommandReceived event.
        /// Also removes the server from the IHandler's DirectoryClose event.
        /// </summary>
        /// <param name="sender">The Object that invoked the event (IHandler) </param>
        /// <param name="args">DirectoryCloseEventArgs</param>
        private void OnCloseServer(Object sender, DirectoryCloseEventArgs args)
        {
            IHandler h = (IHandler)sender;
            CommandReceived -= h.OnCommandReceived;
            h.DirectoryClose -= OnCloseServer;
        }
    }
}
