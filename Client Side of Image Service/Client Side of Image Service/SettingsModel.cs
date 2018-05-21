using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class SettingsModel
    {
        public SettingsInfo info { get; private set; }
        public ObservableCollection<string> handlers { get; private set; }
        private ClientTCP client;

        public event EventHandler<string> handlersUpdated;

        public SettingsModel()
        {
            client = ClientTCP.getInstance();
            if (client.IsConnected())
            {
                ClientTCP.OnMessageReceived += UpdateHandlers;
                ClientTCP.OnMessageReceived += UpdateSettingsMap;
                client.sendCommand(CommandEnum.GetConfigCommand.ToString());
            }
        }

        public void UpdateHandlers(object sender, string args)
        {
            string closeCommand = CommandEnum.CloseCommand.ToString();
            if (!args.StartsWith(closeCommand)) return;
            args = args.TrimStart(closeCommand.ToCharArray());
            handlers.Remove(args);
            handlersUpdated?.Invoke(this, args);
        }

        public void UpdateSettingsMap(object sender, string args)
        {
            string settingsCommand = CommandEnum.GetConfigCommand.ToString();
            if (!args.StartsWith(settingsCommand)) return;
            args = args.TrimStart(settingsCommand.ToCharArray());
            string[] delimiter = { "Handlers:" };
            string[] splittedArgs = args.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            info = new SettingsInfo(splittedArgs[0]);
            handlers = new ObservableCollection<string>(splittedArgs[1].Split(';'));
        }

        public void RemoveHandler(object sender, string args)
        {
            client.sendCommand(CommandEnum.CloseCommand.ToString() + args);
        }
    }
}
