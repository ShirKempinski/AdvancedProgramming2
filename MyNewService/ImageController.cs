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
        private Dictionary<CommandEnum, ICommand> commandsMap;
        private IImageModal imageModal;
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
            commandsMap = new Dictionary<CommandEnum, ICommand>();
            this.imageModal = imageModal;
            SetDictionary();
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
            ICommand command = commandsMap[commandID];
            return command.Execute(args, out result);
        }

        /// <summary>
        /// Sets the Dictionary mapping CommandEnum to the respective ICommand implementation.
        /// </summary>
        private void SetDictionary()
        {
            commandsMap.Add(CommandEnum.NewFileCommand, new NewFileCommand(this.imageModal));
        }
    }
}