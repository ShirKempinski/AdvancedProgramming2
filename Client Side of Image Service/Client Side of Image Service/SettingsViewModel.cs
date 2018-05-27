using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Client_Side_of_Image_Service
{
    /// <summary>
    /// SettingsViewModel 
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region members
        private SettingsModel model;
        public SettingsEntry outputDirectory { get { return model.outputDirectory; } }
        public SettingsEntry logName { get { return model.logName; } }
        public SettingsEntry sourceName { get { return model.sourceName; } }
        public SettingsEntry thumbnailSize { get { return model.thumbnailSize; } }
        public ICommand RemoveCommand { get; private set; }
        public ObservableCollection<string> handlers { get { return model.handlers; } }
        public new event PropertyChangedEventHandler PropertyChanged;
        #endregion
        

        public string ChosenHandler
        {
            get { return model.ChosenHandler; }
            set
            {
                model.ChosenHandler = value;
                NotifyPropertyChanged("chosenHandler");
            }
        }

        /// <summary>
        /// Constructor for the SettingsViewModel
        /// </summary>
        public SettingsViewModel()
        {
            model = new SettingsModel();
            RemoveCommand = new DelegateCommand<object>(OnRemove, IsRemovable);
            PropertyChanged += RemoveHandler;
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged(e.PropertyName);
            };
        }

        /// <summary>
        /// Invokes the RaiseCanExecuteChanged on the RemoveCommand
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">PropertyChangedEeventArgs</param>
        private void RemoveHandler(object sender, PropertyChangedEventArgs e)
        {
            var command = RemoveCommand as DelegateCommand<object>;
            command?.RaiseCanExecuteChanged();

        }

        /// <summary>
        /// Remove the Model ChosenHandler and sets it to null
        /// </summary>
        /// <param name="obj"> Object</param>
        public void OnRemove(object obj)
        {
            model.RemoveHandler(ChosenHandler);
            model.ChosenHandler = null;
        }

        /// <summary>
        /// Indicates an item can be removed (Removable interface)
        /// </summary>
        /// <param name="obj">The object which we want to verify</param>
        /// <returns>boolean, true if removable otherwise false</returns>
        private bool IsRemovable(object obj)
        {
            return handlers.Contains(ChosenHandler);
        }

        /// <summary>
        /// Invokes the PropetyChanged event
        /// </summary>
        /// <param name="name"> String, the name of the changed parameter</param>
        protected new void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
