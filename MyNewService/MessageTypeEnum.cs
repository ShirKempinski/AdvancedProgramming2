using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// Enum that defines the available message types.
    /// <remarks> INFO 0, WARNING 1, FAIL 2</remarks>
    public enum MessageTypeEnum : int
    {
        INFO,
        WARNING,
        FAIL
    }
}
