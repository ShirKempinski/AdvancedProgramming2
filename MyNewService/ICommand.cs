using System;
using System.Collections.Generic;

namespace ImageService
{
    /// <summary>
    /// interface ICommand has an Execute function.
    /// </summary>
    public interface ICommand
    {
        // The Function That will Execute The command
        string Execute(List<String> args, out bool result);
    }
}
