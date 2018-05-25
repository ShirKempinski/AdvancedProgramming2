using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Client_Side_of_Image_Service
{
    public class SettingsViewModel : BaseViewModel
    {
        private SettingsModel model;

        public SettingsEntry outputDirectory { get { return model.outputDirectory; } }
        public SettingsEntry logName { get { return model.logName; } }
        public SettingsEntry sourceName { get { return model.sourceName; } }
        public SettingsEntry thumbnailSize { get { return model.thumbnailSize; } }

        public ObservableCollection<string> handlers { get { return model.handlers; } }

        public SettingsViewModel()
        {
            model = new SettingsModel();
            model.handlersUpdated += RemoveHandler;
        }

        public void RemoveHandler(object sender, List<string> args)
        {
            foreach (string handler in args)
            {
                handlers.Remove(handler);
            }
        }

        public void CloseHandler(string handler)
        {
            model.RemoveHandler(this, handler);
        }
    }
}
