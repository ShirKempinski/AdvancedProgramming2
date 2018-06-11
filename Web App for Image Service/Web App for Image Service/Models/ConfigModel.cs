using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

namespace Web_App_for_Image_Service.Models
{
    public class ConfigModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Output Directory")]
        public string outputDirectory { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Source Name")]
        public string sourceName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Name")]
        public string logName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Thumbnail Size")]
        public string thumbnailSize { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Handlers")]
        public List<string> handlers { get; private set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Waiting Mode")]
        public bool enabled;

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Selected Handler")]
        public string selectedHandler;

        private ClientTCP client;

        public ConfigModel()
        {
            handlers = new List<string>();
            client = ClientTCP.getInstance();
            if (client.isConnected)
            {
                ClientTCP.OnMessageReceived += UpdateHandlers;
                ClientTCP.OnMessageReceived += UpdateConfigMap;
                client.sendCommand(CommandEnum.GetConfigCommand.ToString());
                enabled = true;
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
            enabled = true;
        }
    
        public void UpdateConfigMap(object sender, List<string> args)
        {
            string ConfigCommand = CommandEnum.GetConfigCommand.ToString();
            if (args[0] != ConfigCommand) return;
            args.Remove(args[0]);
            string handlersString = args.First(line => (line.StartsWith("Handlers:")));
            handlersString = handlersString.TrimStart("Handlers:".ToCharArray());
            handlers = new List<string>(handlersString.Split(';'));
            UpdateEntries(args);
        }

        public void RemoveHandler(string handler)
        {
            client.sendCommand(CommandEnum.CloseCommand.ToString() + handler);
            enabled = false;
        }

        public void UpdateEntries(List<string> args)
        {
            outputDirectory = args.First(line => (line.StartsWith("OutputDir:"))).TrimStart("OutputDir:".ToCharArray());
            sourceName = args.First(line => (line.StartsWith("SourceName:"))).TrimStart("SourceName:".ToCharArray());
            logName = args.First(line => (line.StartsWith("LogName:"))).TrimStart("LogName:".ToCharArray());
            thumbnailSize = args.First(line => (line.StartsWith(
                "ThumbnailSize:"))).TrimStart("ThumbnailSize:".ToCharArray());
        }
    }
}
