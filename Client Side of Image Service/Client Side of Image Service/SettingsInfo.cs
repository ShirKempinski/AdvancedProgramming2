using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class SettingsInfo
    {
        public string outputDirectory { get; set; }
        public string sourceName { get; set; }
        public string logName { get; set; }
        public string thumbnailSize { get; set; }

        public SettingsInfo(List<string> args)
        {
            if (args == null) { return; }            
            outputDirectory = args[0];
            sourceName = args[1];
            logName = args[2];
            thumbnailSize = args[3];
        }
    }
}
