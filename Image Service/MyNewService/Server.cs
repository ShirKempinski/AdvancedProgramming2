﻿using System;
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
        public List<TcpClient> connectedClients { get; private set; }
        public TcpListener listener { get; set; }
        public Thread listeningThread { get; set; }
        private Mutex clientsMutex { get; set; }
        public bool shouldStop { get; set; }

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
            connectedClients = new List<TcpClient>();
        }

        /// <summary>
        /// Starts listening to changes in the direcotries specified on App.config.
        /// </summary>
        /// <remarks>
        /// Creates an IHandler per target directory.
        /// </remarks>
        public void Start()
        {
            string handlers = ConfigurationManager.AppSettings["Handlers"];
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
//            h.DirectoryClose += OnCloseServer;
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
            shouldStop = true;
            CloseAllClients();
        }

        private static void HandleClient(Server server, TcpClient client)
        {
            Stream clientStream = new NetworkStream(client.Client);
            StreamWriter clientWriter = new StreamWriter(clientStream);
            clientWriter.AutoFlush = true;
            StreamReader clientReader = new StreamReader(clientStream);
            try
            {                
                while (client.Connected)
                {
                    // read the message
                    string message = clientReader.ReadLine();
                    // if client disconnected, break out
                    if (message == "closeClient") break;
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
                        server.logger.Log("LogCommand received", MessageTypeEnum.INFO);
                        ICommand logCommand = new LogCommand(server.logger);
                        logCommand.Execute(toSend, out bool res);
                    }
                    else if (message.StartsWith(CommandEnum.GetConfigCommand.ToString()))
                    {
                        server.logger.Log("ConfigCommand received", MessageTypeEnum.INFO);
                        ICommand getConfigCommand = new GetConfigCommand();
                        getConfigCommand.Execute(toSend, out bool res);
                    }
                    if (toSend.Count == 0) continue;

                    //send the client it's request
                    toSend.Add("<EOF>");

                    server.clientsMutex.WaitOne();
                    foreach (string s in toSend)
                    {
                        clientWriter.WriteLine(s);
                        server.logger.Log(s + " was sent", MessageTypeEnum.INFO);
                    }
                    server.clientsMutex.ReleaseMutex();
                }
                server.logger.Log("Client disconnected", MessageTypeEnum.INFO);
                server.connectedClients.Remove(client);
            } catch (Exception e)
            {
                server.logger.Log(e.Message, MessageTypeEnum.FAIL);
                if (client.Connected) client.Close();
                server.connectedClients.Remove(client);
                server.logger.Log("Client disconnected and has been removed", MessageTypeEnum.INFO);
            }
        }

        private void CloseAllClients()
        {
            clientsMutex.WaitOne();
            foreach(TcpClient client in connectedClients)
            {
                client.Close();
                connectedClients.Remove(client);
            }
            clientsMutex.ReleaseMutex();
        }

        public static void StartListening(Server server)
        {
           server.shouldStop = false;  
            try
            {
                // Bind the socket to the local endpoint and listen for incoming connections.  
                server.listener = new TcpListener(IPAddress.Parse(ConfigurationManager.AppSettings["IP"]),
                int.Parse(ConfigurationManager.AppSettings["Port"]));
                // Start listening for connections.
                server.listeningThread = new Thread(() =>
                {
                    server.logger.Log("Start listening", MessageTypeEnum.INFO);
                    server.listener.Start();
                    while (!server.shouldStop)
                    {
                        // Program is suspended while waiting for an incoming connection.
                        if (server.listener.Pending())
                        {
                            server.logger.Log("client connected", MessageTypeEnum.INFO);
                            TcpClient client = server.listener.AcceptTcpClient();
                            server.clientsMutex.WaitOne();
                            server.connectedClients.Add(client);
                            server.clientsMutex.ReleaseMutex();
                            Thread clientThread = new Thread(() => HandleClient(server, client));
                            clientThread.Start();
                        }
                    }
                    server.logger.Log("Stopped listening", MessageTypeEnum.INFO);
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
            foreach (TcpClient client in connectedClients)
            {
                StreamWriter writer = new StreamWriter(new NetworkStream(client.Client));
                writer.AutoFlush = true;
                writer.WriteLine(CommandEnum.CloseCommand);
                writer.WriteLine(handlerPath);
                writer.WriteLine("<EOF>");
                writer.Close();
            }
            clientsMutex.ReleaseMutex();
        }
    }
}
