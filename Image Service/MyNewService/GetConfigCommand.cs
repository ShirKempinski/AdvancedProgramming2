using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class GetConfigCommand: ICommand
    {
        public GetConfigCommand() { }

        string ICommand.Execute(List<string> args, out bool result)
        {
            args.Add("OutputDir:" + ConfigurationManager.AppSettings["OutputDir"]);
            args.Add("SourceName:" + ConfigurationManager.AppSettings["SourceName"]);
            args.Add("ThumbnailSize:" + ConfigurationManager.AppSettings["ThumbnailSize"]);
            args.Add(ConfigurationManager.AppSettings["LogName"]);
            args.Add(ConfigurationManager.AppSettings["Handler"]);
            result = true;
            return "Configuration Requested";
        }
    }
}
