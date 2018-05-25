﻿using System;
using System.Collections.Generic;
using System.Configuration;

namespace ImageService
{
    public class GetConfigCommand: ICommand
    {
        public GetConfigCommand() { }

        string ICommand.Execute(List<string> args, out bool result)
        {
            args.Add("OutputDir:" + ConfigurationManager.AppSettings["OutputDir"]);
            args.Add("SourceName:" + ConfigurationManager.AppSettings["SourceName"]);
            args.Add("LogName:" + ConfigurationManager.AppSettings["LogName"]);
            args.Add("ThumbnailSize:" + ConfigurationManager.AppSettings["ThumbnailSize"]);
            args.Add("Handlers:" + ConfigurationManager.AppSettings["Handlers"]);
            args.Insert(0, CommandEnum.GetConfigCommand.ToString());
            result = true;
            return "Configuration Requested";
        }
    }
}
