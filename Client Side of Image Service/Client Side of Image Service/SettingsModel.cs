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

        public event EventHandler<List<string>> handlersUpdated;

        public SettingsModel()
        {
            client = ClientTCP.getInstance();
            if (client.IsConnected())
            {
                ClientTCP.OnMessageReceived += UpdateHandlers;
                ClientTCP.OnMessageReceived += UpdateSettingsMap;
                client.sendCommand(CommandEnum.GetConfigCommand.ToString());
            }
            info = new SettingsInfo(null);
        }

        public void UpdateHandlers(object sender, List<string> args)
        {
            string closeCommand = CommandEnum.CloseCommand.ToString();
            if (args[0] != closeCommand) return;
            args.Remove(args[0]);
            foreach (string handler in args)
            {
                handlers.Remove(handler);
            }
            handlersUpdated?.Invoke(this, args);
        }

        public void UpdateSettingsMap(object sender, List<string> args)
        {
            string settingsCommand = CommandEnum.GetConfigCommand.ToString();
            if (args[0] != settingsCommand) return;
            args.Remove(args[0]);
            string handlersString = args.Last();
            args.Remove(handlersString);
            info = new SettingsInfo(args);
            handlers = new ObservableCollection<string>(handlersString.Split(';'));
        }

        public void RemoveHandler(object sender, string args)
        {
            client.sendCommand(CommandEnum.CloseCommand.ToString() + args);
        }
    }
}
