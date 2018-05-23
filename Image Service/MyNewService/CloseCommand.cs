using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class CloseCommand: ICommand
    {
        string ICommand.Execute(List<string> args, out bool result)
        {
            string handlers = ConfigurationManager.AppSettings["Handler"];
            string[] handlerNames = handlers.Split(';');
            string newHandlers = "";
            foreach(string s in handlerNames)
            {
                if (s != args[0])
                {
                    newHandlers += (s + ";");
                }
            }
            newHandlers.TrimEnd(';');
            ConfigurationManager.AppSettings["Handler"] = newHandlers;
            args[0] = newHandlers;
            result = true;
            return "Configuration Requested";
        }
    }
}
