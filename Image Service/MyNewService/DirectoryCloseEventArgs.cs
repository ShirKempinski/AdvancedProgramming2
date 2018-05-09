using System;

namespace ImageService
{
    /// <summary>
    /// DirectoryCloseEventArgs class. Contains the command message and the associated path.
    /// </summary>
    /// <remarks>
    /// Implements EventArgs Interface
    /// </remarks>
    public class DirectoryCloseEventArgs : EventArgs
    {
        #region Members
        public string DirectoryPath { get; set; }
        // The Message That goes to the logger
        public string Message { get; set; }
        #endregion

        /// <summary>
        /// DirectoryCloseEventArgs constructor.
        /// </summary>
        /// <param name="dirPath"> command's associated path </param>
        /// <param name="message"> command's message </param>
        /// <returns>
        /// A new DirectoryCloseEventArgs object
        /// </returns>
        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            // Setting the Directory Name
            DirectoryPath = dirPath;
            // Storing the String
            Message = message;
        }
    }
}
