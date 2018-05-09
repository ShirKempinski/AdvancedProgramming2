using System;
using System.Collections.Generic;

namespace ImageService
{
    /// <summary>
    /// class NewFileCommand. Generated and used when a new file is created.
    /// </summary>
    /// <remarks>
    /// Implements the ICommand interface.
    /// </remarks>
    public class NewFileCommand : ICommand
    {
        #region Members
        private IImageModal imageModal;
        #endregion

        /// <summary>
        /// NewFileCommand constructor.
        /// </summary>
        /// <param name="modal"> IImageModal object </param>
        /// <returns> new NewFileCommand Object</returns>
        public NewFileCommand(IImageModal modal)
        {
            // Storing the Modal
            imageModal = modal;
        }

        /// <summary>
        /// ICommand Method. Runs the AddFile Command on the provided args.
        /// </summary>
        /// <param name="args">List<String> the necessary arguments for the ICommand execution </param>
        /// <param name="result">boolean, if the File was added and the Thumbail created succesfully or not</param>
        /// <returns>The String Will Return the New Path if result = true, else will return the error message</returns>
        public string Execute(List<String> args, out bool result)
        {
            return imageModal.AddFile(args, out result);
        }
    }
}
