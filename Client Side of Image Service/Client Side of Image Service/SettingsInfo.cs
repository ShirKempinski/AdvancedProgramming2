using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class SettingsInfo
    {
        public string outputDirectory { get; }
        public string sourceName { get; }
        public string logName { get; }
        public string thumbnailSize { get; }

        public SettingsInfo(string args)
        {
            string[] delimiter = { "Output Directory:", "Source Name:", "Log Name:", "Thumbnail Size:" };
            string[] configInfo = args.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            outputDirectory = configInfo[0];
            sourceName = configInfo[1];
            logName = configInfo[2];
            thumbnailSize = configInfo[3];
        }
    }
}
