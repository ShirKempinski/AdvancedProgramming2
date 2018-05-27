using System;
using System.Collections.Generic;

namespace ImageService
{
    /// <summary>
    /// LoggingModal Class, an implementation of the ILogging Interface.
    /// </summary>
    class LoggingModal : ILogging
    {
        #region Members
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<LogEntriesEventArgs> GetEntries;
        #endregion

        /// <summary>
        /// LoggingModal constructor.
        /// </summary>
        /// <returns> new LoggingModal object</returns>
        public LoggingModal() { }

        /// <summary>
        /// ILoggingModal method. Logs a message and its Status to the Service's Log.
        /// </summary>
        /// <param name="message"> String the message to be logged </param>
        /// <param name="type"> MessageTypeEnum </param>
        /// <remarks> Logging is done through the MessageReceived Event</remarks>
        void ILogging.Log(string message, MessageTypeEnum type)
        {
            MessageReceivedEventArgs args = new MessageReceivedEventArgs();
            if (message.Contains(Environment.NewLine)) message = message.Replace(Environment.NewLine, " ");
            args.Message = message;
            args.Status = type;
            MessageReceived?.Invoke(this, args);
        }

        List<string> ILogging.EntriesRequest()
        {
            LogEntriesEventArgs args = new LogEntriesEventArgs();
            GetEntries?.Invoke(this, args);
            return args.Args;
        }
    }
}
