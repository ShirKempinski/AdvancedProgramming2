using System;
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
        private ILogging logger;
        private IController controller;
        private static volatile List<Socket> connectedClients;
        private static Mutex clientsMutex;

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
            CommandCentral.SetCommands(logger);
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
            StartListening();
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
            CloseAllClients();
        }

        private static void HandleClient(object socket)
        {
            Socket clientSocket = (Socket) socket;
            while (true)
            {
                byte[] bytes = new Byte[1024];
                string data = null;
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = clientSocket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
                if (data == "closeClient") { break; }
                CommandEnum @enum = CommandCentral.getCommandEnum(data);
                List<string> toSend = new List<string>();
                CommandCentral.getCommand(@enum).Execute(toSend, out bool result);
                toSend.Add("<EOF>");
                foreach (string s in toSend)
                {
                    byte[] msg = Encoding.ASCII.GetBytes(s);
                    clientSocket.Send(msg);
                }
            }
            
        }

        private void CloseAllClients()
        {
            clientsMutex.WaitOne();
            foreach(Socket sckt in connectedClients)
            {
                sckt.Close();
                connectedClients.Remove(sckt);
            }
            clientsMutex.ReleaseMutex();
        }

        public static void StartListening()
        {
            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                // Start listening for connections.  
                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    clientsMutex.WaitOne();
                    connectedClients.Add(handler);
                    clientsMutex.ReleaseMutex();
                    Thread clientThread = new Thread(() => HandleClient(handler));
                    clientThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
