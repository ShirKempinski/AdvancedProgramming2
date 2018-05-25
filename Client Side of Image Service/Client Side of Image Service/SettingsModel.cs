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
        public SettingsEntry outputDirectory { get; set; }
        public SettingsEntry sourceName { get; set; }
        public SettingsEntry logName { get; set; }
        public SettingsEntry thumbnailSize { get; set; }
        public ObservableCollection<string> handlers { get; private set; }
        private ClientTCP client;

        public event EventHandler<List<string>> handlersUpdated;

        public SettingsModel()
        {
            client = ClientTCP.getInstance();
            if (client.isConnected)
            {
                ClientTCP.OnMessageReceived += UpdateHandlers;
                ClientTCP.OnMessageReceived += UpdateSettingsMap;
                outputDirectory = new SettingsEntry(null);
                sourceName = new SettingsEntry(null);
                logName = new SettingsEntry(null);
                thumbnailSize = new SettingsEntry(null);
                client.sendCommand(CommandEnum.GetConfigCommand.ToString());
            }
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
            string handlersString = args.First(line =>(line.StartsWith("Handlers:")));
            handlersString = handlersString.TrimStart("Handlers:".ToCharArray());
            handlers = new ObservableCollection<string>(handlersString.Split(';'));
            UpdateEntries(args);
        }

        public void RemoveHandler(object sender, string args)
        {
            client.sendCommand(CommandEnum.CloseCommand.ToString() + args);
        }

        public void UpdateEntries(List<string> args)
        {
            outputDirectory = new SettingsEntry(args.First(line =>
                                    (line.StartsWith("OutputDir:"))).TrimStart("OutputDir:".ToCharArray()));
            sourceName = new SettingsEntry(args.First(line =>
                                    (line.StartsWith("SourceName:"))).TrimStart("SourceName:".ToCharArray()));
            logName = new SettingsEntry(args.First(line =>
                                    (line.StartsWith("LogName:"))).TrimStart("LogName:".ToCharArray()));
            thumbnailSize = new SettingsEntry(args.First(line =>
                                    (line.StartsWith("ThumbnailSize:"))).TrimStart("ThumbnailSize:".ToCharArray()));
        }
    }
}
