using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public sealed class ClientTCP
    {
        private static TcpClient client;
        private static StreamReader reader;
        private static StreamWriter writer;
        private static Stream stream;
        private static bool isConnected;
        private static volatile ClientTCP instance;
        private static Mutex sendAndReceiveMutex;
        private static Object locker = new object();
        private Thread receiver; 


        public static event EventHandler<List<string>> OnMessageReceived;

        private ClientTCP()
        {
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPort = ConfigurationManager.AppSettings["ServerPort"];
            sendAndReceiveMutex = new Mutex();
            StartClient(serverIP, serverPort);
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        private void StartClient(string IP, string port)
        {
            // Connect to a remote device.  
            try
            {
                client = new TcpClient(IP, int.Parse(port));//client = new TcpClient(new IPEndPoint(IPAddress.Parse(IP), int.Parse(port)));
                //client.Connect(IP, int.Parse(port));
                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                isConnected = true;
                receiver = new Thread(getMessage);
                writer.WriteLine("Client started!");
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
            sendAndReceiveMutex.WaitOne();
            writer.WriteLine(command);
            sendAndReceiveMutex.ReleaseMutex();
            Console.WriteLine("Client sent a message! Message: " + command);

        }

        public static void getMessage()
        {
            while (isConnected)
            {
                string message;
                List<string> info = new List<string>();
                while ((message = reader.ReadLine()) != "<EOF>")
                {
                    if (message.Length == 0) continue;
                    info.Add(message);
                }
                writer.WriteLine("Client received a message! Message: " + message);
                Console.WriteLine("Client received a message! Message: " + message);
                OnMessageReceived?.Invoke(getInstance(), info);    
            }
        }
    }
}
