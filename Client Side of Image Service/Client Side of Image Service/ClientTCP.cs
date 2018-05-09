using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class ClientTCP
    {
        private Socket socket;
        private bool isConnected;
        private static ClientTCP instance;
        private static Mutex instanceMutex;
        
        public event EventHandler<string> OnMessageReceived;

        private ClientTCP()
        {
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPort = ConfigurationManager.AppSettings["ServerPort"];
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
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(port);

                // Create a TCP/IP socket.  
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                socket.BeginConnect(remoteEP, null, socket);

                isConnected = true;
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
            socket.Send(message);
        }

        public string getMessage()
        {
            
        }
    }
}
