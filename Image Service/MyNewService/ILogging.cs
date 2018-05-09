using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// ILogging interface has a MessageReceived event and a Log function.
    /// </summary>
    public interface ILogging
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<LogEntriesEventArgs> GetEntries;

        // Logging the Message
        void Log(String message, MessageTypeEnum type);
        List<string> EntriesRequest();
    }
}
