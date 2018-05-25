using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client_Side_of_Image_Service
{
    public class MainWindowController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Brush _backgroundColor;
        public Brush backgroundColor {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor == value) return;
                _backgroundColor = value;
                OnPropertyChanged("backgroundColor");
            }
        }

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public MainWindowController()
        {
            ClientTCP client = ClientTCP.getInstance();
            client.connectionStatusUpdated += delegate (object sender, PropertyChangedEventArgs args)
            {
                App.Current.Dispatcher.Invoke(delegate
                {
                    if (args.PropertyName == "disconnected") backgroundColor = Brushes.Gray;
                    else backgroundColor = Brushes.Azure;
                });
            };
        }
    }
}
