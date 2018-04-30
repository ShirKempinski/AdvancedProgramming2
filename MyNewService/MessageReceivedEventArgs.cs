using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// EventArgs for the MessageReceived Event
    /// </summary>
    /// <remarks> Members: Status - MessageTypeEnum, Message - String</remarks>
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }
    }
}
