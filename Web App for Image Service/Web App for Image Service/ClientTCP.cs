using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Web_App_for_Image_Service
{
    public sealed class ClientTCP
    {
        #region Members
        public TcpClient client { get; set; }
        public StreamReader reader { get; set; }
        public StreamWriter writer { get; set; }
        public Stream stream { get; set; }
        private static volatile ClientTCP instance;
        public Mutex sendAndReceiveMutex { get; set; }
        public static Object locker = new object();
        //private IConfigurationRoot configuration;
        public Thread receiver { get; set; }
        public event PropertyChangedEventHandler connectionStatusUpdated;
        private bool _isConnected;
        public bool isConnected
        {
            get
            {
                if (client != null)
                {
                    if (_isConnected != client.Connected) _isConnected = client.Connected;
                    return _isConnected;
                }
                else return false;
            }
            set
            {
                _isConnected = value;
                string msg;
                if (value == true) msg = "connected";
                else msg = "disconnected";
                connectionStatusUpdated?.Invoke(this, new PropertyChangedEventArgs(msg));
            }
        }
 
        public static event EventHandler<List<string>> OnMessageReceived;
        #endregion


        //private ClientTCP(IConfiguration _config)
        private ClientTCP()
        {
            /*
            string serverIP = _config["ServerIP"];
            string serverPort = _config["ServerPort"];
            */
            string serverIP = "127.0.0.1";
            string serverPort = "11000";
            sendAndReceiveMutex = new Mutex();
            StartClient(serverIP, serverPort);
        }

        private void StartClient(string IP, string port)
        {
            // Connect to a remote device.  
            try
            {
                client = new TcpClient(IP, int.Parse(port));
                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                isConnected = client.Connected;
                writer.AutoFlush = true;
                receiver = new Thread(getMessage);
                receiver.Start();
                Console.WriteLine("Client started!");
            }
            catch (Exception e)
            {
                if (client != null) client.Close();
                isConnected = false;
                Console.WriteLine(e.Message);
            }
        }

        public static ClientTCP getInstance()
        {
            if (instance == null)
            {
                lock(locker)
                {
                    if (instance == null)
                    {
                        instance = new ClientTCP();
                    }
                }
            }
            return instance;
        }

        public void sendCommand (string command)
        {
            if (!isConnected) return;
            sendAndReceiveMutex.WaitOne();
            writer.WriteLine(command);
            sendAndReceiveMutex.ReleaseMutex();
            Console.WriteLine("Client sent a message! Message: " + command);

        }

        public void getMessage()
        {
            try
            {
                while (isConnected)
                {
                    string message = reader.ReadLine();
                    List<string> info = new List<string>();
                    while (message != "<EOF>")
                    {
                        if (string.IsNullOrEmpty(message)) continue;
                        info.Add(message);
                        Console.WriteLine("got a message: " + message);
                        message = reader.ReadLine();
                    }
                    OnMessageReceived?.Invoke(this, info);
                }
            } catch (Exception e)
            {
                if (client != null) client.Close();
                isConnected = false;
                Console.WriteLine(e.Message);
            }
        }

        public void Disconnect()
        {
            sendCommand("closeClient");
            client.Close();
            isConnected = false;
        }
    }
}
