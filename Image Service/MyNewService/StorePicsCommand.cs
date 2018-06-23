using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ImageService
{
    public class StorePicsCommand : ICommand
    {
        private TcpClient client;
        private Dictionary<string, Image> pictures;
        private static Mutex mutex;

        public StorePicsCommand(TcpClient client)
        {
            this.client = client;
            pictures = new Dictionary<string, Image>();
            if (mutex == null) mutex = new Mutex();
        }

        public string Execute(List<string> args, out bool result)
        {
            result = true;
            ReadImages();
            SaveImages();
            return null;
        }

        private void SaveImages()
        {
            string handlers = ConfigurationManager.AppSettings["Handlers"];
            if (handlers == null) return;
            string path = handlers.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            foreach (string name in pictures.Keys)
            {
                pictures[name].Save(Path.Combine(path, name));
            }
        }

        private void ReadImages()
        {
            NetworkStream stream = new NetworkStream(client.Client);
            BinaryReader reader = new BinaryReader(stream);
            mutex.WaitOne();
            int numOfPics = reader.ReadInt32();
            for (int i = 0; i < numOfPics; i++)
            {
                int nameSize = reader.ReadInt32();
                string name = Encoding.UTF8.GetString(reader.ReadBytes(nameSize));
                int numOfBytes = reader.ReadInt32();
                byte[] bytes = reader.ReadBytes(numOfBytes);
                Image picture = byteArrayToImage(bytes);
                pictures.Add(name, picture);
            }
            mutex.ReleaseMutex();
        }

        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}