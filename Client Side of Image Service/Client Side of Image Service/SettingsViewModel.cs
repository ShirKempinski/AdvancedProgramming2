using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Client_Side_of_Image_Service
{
    public class SettingsViewModel : BaseViewModel
    {
        private SettingsModel model;

        public string outputDirectory { get { return model.info.outputDirectory; } }
        public string logName { get { return model.info.logName; } }
        public string sourceName { get { return model.info.sourceName; } }
        public string thumbnailSize { get { return model.info.thumbnailSize; } }

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
