using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client_Side_of_Image_Service
{
    public class SettingsViewModel : BaseViewModel
    {
        private SettingsModel model;

        public SettingsEntry outputDirectory { get { return model.outputDirectory; } }
        public SettingsEntry logName { get { return model.logName; } }
        public SettingsEntry sourceName { get { return model.sourceName; } }
        public SettingsEntry thumbnailSize { get { return model.thumbnailSize; } }
        public ICommand RemoveCommand { get; private set; }
        public ObservableCollection<string> handlers { get { return model.handlers; } }

        public new event PropertyChangedEventHandler PropertyChanged;

        public string ChosenHandler
        {
            get { return model.ChosenHandler; }
            set
            {
                model.ChosenHandler = value;
                NotifyPropertyChanged("chosenHandler");
            }
        }

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

        private void RemoveHandler(object sender, PropertyChangedEventArgs e)
        {
            var command = RemoveCommand as DelegateCommand<object>;
            command?.RaiseCanExecuteChanged();

        }

        public void OnRemove(object obj)
        {
            model.RemoveHandler(ChosenHandler);
            model.ChosenHandler = null;
        }

        private bool IsRemovable(object obj)
        {
            return handlers.Contains(ChosenHandler);
        }

        protected new void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
