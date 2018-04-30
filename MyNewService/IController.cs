using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// Interface IController has an ExecuteCommand function.
    /// </summary>
    public interface IController
    {
        string ExecuteCommand(CommandEnum commandID, List<String> args, out bool result);
    }
}
