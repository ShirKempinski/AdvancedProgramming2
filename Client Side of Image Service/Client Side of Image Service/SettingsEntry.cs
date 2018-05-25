using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side_of_Image_Service
{
    public class SettingsEntry : INotifyPropertyChanged
    {
        private string _entry;
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string msg)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(msg));
        }

        public string entry
        {
            get { return _entry; }
            set
            {
                if (string.IsNullOrEmpty(value)) _entry = "Waiting for information from server";
                else if (_entry != value) _entry = value;
                OnPropertyChanged(value);
            }
        }        

        public SettingsEntry(string s)
        {
            if (string.IsNullOrEmpty(s)) _entry = "Waiting for information from server";
            else _entry = s;
        }
    }
}
