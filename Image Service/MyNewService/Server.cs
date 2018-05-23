using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        public ILogging logger { get; }
        public IController controller { get; }
        public static List<Socket> connectedClients { get; private set; }
        public static TcpListener listener { get; set; }
        public Thread listeningThread { get; set; }
        private static Mutex clientsMutex { get; set; }

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
            clientsMutex = new Mutex();
            connectedClients = new List<Socket>();
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
            StartListening(this);
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
            CommandReceived?.Invoke(this, cArgs);
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
            CloseAllClients();
        }

        private static void HandleClient(Server server, Socket client)
        {
            Stream clientStream = new NetworkStream(client);
            StreamWriter clientWriter = new StreamWriter(clientStream);
            StreamReader clientReader = new StreamReader(clientStream);
            while (true)
            {
                // read the message
                string message = clientReader.ReadLine();
                // if client disconnected, break out
                if (message == "closeClient") break;
                server.logger.Log(message, MessageTypeEnum.INFO);
                List<string> toSend = new List<string>();
                if (message.StartsWith(CommandEnum.CloseCommand.ToString()))
                {
                    string path = message.TrimStart(CommandEnum.CloseCommand.ToString().ToCharArray());
                    server.SendCommand(CommandEnum.CloseCommand, null, path);
                    //update clients
                    server.HandlersChanged(path);
                    continue;
                }
                if (message.StartsWith(CommandEnum.LogCommand.ToString()))
                {
                    server.logger.Log("LogCommand commanded", MessageTypeEnum.INFO);
                    ICommand logCommand = new LogCommand(server.logger);
                    logCommand.Execute(toSend, out bool res);
                }
                else if (message.StartsWith(CommandEnum.GetConfigCommand.ToString()))
                {
                    server.logger.Log("Config commanded", MessageTypeEnum.INFO);
                    ICommand getConfigCommand = new GetConfigCommand();
                    getConfigCommand.Execute(toSend, out bool res);
                }
              
                //send the client it's request
                toSend.Add("<EOF>\n");

                clientsMutex.WaitOne();
                foreach (string s in toSend)
                {
                    clientWriter.WriteLine(s);
                }
                clientsMutex.ReleaseMutex();
                server.logger.Log(toSend[0] + " was sent" , MessageTypeEnum.INFO);
            }
            clientStream.Close();
        }

        private void CloseAllClients()
        {
            clientsMutex.WaitOne();
            foreach(Socket client in connectedClients)
            {
                client.Close();
                connectedClients.Remove(client);
            }
            clientsMutex.ReleaseMutex();
        }

        public static void StartListening(Server server)
        {
            listener = new TcpListener(IPAddress.Parse(ConfigurationManager.AppSettings["IP"]),
                int.Parse(ConfigurationManager.AppSettings["Port"]));
            
            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                
                // Start listening for connections.
                server.listeningThread = new Thread(() =>
                {
                    listener.Start();
                    while (true)
                    {                     
                        // Program is suspended while waiting for an incoming connection.
                        if (listener.Pending())
                        {
                            Socket client = listener.AcceptSocket();
                            clientsMutex.WaitOne();
                            connectedClients.Add(client);
                            clientsMutex.ReleaseMutex();
                            Thread clientThread = new Thread(() => HandleClient(server, client));
                            clientThread.Start();
                        }
                    }
                });
                server.listeningThread.Start();
            }
            catch (Exception e)
            {
                server.logger.Log(e.ToString(), MessageTypeEnum.FAIL);
            }
        }

        public void HandlersChanged(string handlerPath)
        {
            clientsMutex.WaitOne();
            foreach (Socket client in connectedClients)
            {
                StreamWriter writer = new StreamWriter(new NetworkStream(client));
                writer.WriteLine(CommandEnum.CloseCommand);
                writer.WriteLine(handlerPath);
                writer.WriteLine("<EOF>");
                writer.Close();
            }
            clientsMutex.ReleaseMutex();
        }
    }
}
