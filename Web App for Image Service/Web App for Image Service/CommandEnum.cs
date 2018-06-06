using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_App_for_Image_Service
{
    public enum CommandEnum: int
    {
        NewFileCommand,
        GetConfigCommand,
        LogCommand,
        CloseCommand,
        CloseServerCommand,
        GetPicsNumCommand,
        DeletePicCommand
    }
}
