using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class ClientTCP
    {
        private static Socket socket;
        private static bool isConnected;
        private static ClientTCP instance;
        private static Mutex instanceMutex;
        private static Mutex sendAndReceiveMutex;
        private Thread receiver; 
        
        public static event EventHandler<string> OnMessageReceived;

        private ClientTCP()
        {
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPort = ConfigurationManager.AppSettings["ServerPort"];
            instanceMutex = new Mutex();
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
                // Establish the remote endpoint for the socket.  
                IPAddress ipAddress = IPAddress.Parse(IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(port));

                // Create a TCP/IP socket.  
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                socket.BeginConnect(remoteEP, null, socket);

                isConnected = true;
                receiver = new Thread(getMessage);
            }
            catch (Exception e)
            {
                socket.Close();
                isConnected = false;
            }
        }

        public static ClientTCP getInstance()
        {
            instanceMutex.WaitOne();
            if (instance == null)
            {
                instance = new ClientTCP();
            }
            instanceMutex.ReleaseMutex();
            return instance;
        }

        public void sendCommand (string command)
        {
            byte[] message = Encoding.ASCII.GetBytes(command);
            sendAndReceiveMutex.WaitOne();
            socket.Send(message);
            sendAndReceiveMutex.ReleaseMutex();
        }

        public static void getMessage()
        {
            while (isConnected)
            {
                byte[] buffer = new byte[2048];
                sendAndReceiveMutex.WaitOne();
                int receivedDataLength = socket.Receive(buffer);
                sendAndReceiveMutex.ReleaseMutex();
                string message = Encoding.ASCII.GetString(buffer, 0, receivedDataLength);
                OnMessageReceived?.Invoke(getInstance(), message);
            }
        }
    }
}
