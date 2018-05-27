using System;
using System.ComponentModel;
using System.Windows.Input;
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
        //public ICommand CloseWindowCommand { get; private set; }

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public MainWindowController()
        {
            
            ClientTCP client = ClientTCP.getInstance();
            try
            {
                client.connectionStatusUpdated += delegate (object sender, PropertyChangedEventArgs args)
                {
                    App.Current.Dispatcher.Invoke(delegate
                    {
                        if (args.PropertyName == "disconnected") backgroundColor = Brushes.Gray;
                        else backgroundColor = Brushes.Turquoise;
                    });
                };
                if (client.isConnected) client.isConnected = true;
                else client.isConnected = false;

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /*
        private bool CanClose(object arg)
        {
            return true; // Allowing the user to close the window always
        }

        private void OnCloseWindow(object sender,EventArgs e)
        {
            ClientTCP.getInstance().Disconnect();
        }
        */
    }
}
