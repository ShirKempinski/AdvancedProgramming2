using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Side_of_Image_Service
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isConnected;
        private Brush background;
        private LogPage logPage;
        private SettingsPage settingsPage;

        public MainWindow()
        {
            isConnected = ClientTCP.getInstance().IsConnected();
            background = isConnected? Brushes.White :  Brushes.Gray;
              if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                logPage = new LogPage();
                settingsPage = new SettingsPage();
            }
            InitializeComponent();
          
        }
    }
}
