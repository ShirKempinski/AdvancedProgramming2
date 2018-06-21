using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Sockets;

namespace ImageService
{
    public class StorePicsCommand : ICommand
    {
        private TcpClient client;
        private Dictionary<string, Image> pictures;

        public StorePicsCommand(TcpClient client)
        {
            this.client = client;
            pictures = new Dictionary<string, Image>();
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
            StreamReader reader = new StreamReader(stream);

            int numOfPics = int.Parse(reader.ReadLine());
            for (int i = 0; i < numOfPics; i++)
            {
                string name = reader.ReadLine();
                int numOfBytes = int.Parse(reader.ReadLine());
                byte[] bytes = new byte[numOfBytes];
                stream.Read(bytes, 0, numOfBytes);
                Image picture = byteArrayToImage(bytes);
                pictures.Add(name, picture);
            }
        }

        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}