using System;
using System.Collections.Generic;

namespace ImageService
{
    /// <summary>
    /// ImageController class.
    /// Responsible for translating and running the CommandEnums relevant to Images into appropriate ICommands.
    /// </summary>
    /// <remarks>Implements the IController interface</remarks>
    class ImageController : IController
    {
        #region Members
        private IImageModal imageModal;
        private ICommand newFileCommand;
        #endregion

        /// <summary>
        /// ImageController constructor.
        /// </summary>
        /// <param name="imageModal">An object of IImageModal type</param>
        /// <returns>
        /// A new ImageController object
        /// </returns>
        public ImageController(IImageModal imageModal)
        {
            this.imageModal = imageModal;
            newFileCommand = new NewFileCommand(imageModal);
        }

        /// <summary>
        /// Runs the ICommand defined by the CommandEnum Execute method on the provided args.
        /// </summary>
        /// <param name="commandID">The ICommand to be executed Enum</param>
        /// <param name="args">The args List<String> taken by the Execute method</param>
        /// <param name="result">True if succeeded, false otherwise</param>
        /// <returns>
        /// The string that results from ICommand Execute method.
        /// </returns>
        /// <remarks>IController method implementation</remarks>
        public string ExecuteCommand(CommandEnum commandID, List<String> args, out bool result)
        {
            if (commandID == CommandEnum.NewFileCommand)
            {
                return newFileCommand.Execute(args, out result);
            }
            result = false;
            return null;
        }
    }
}