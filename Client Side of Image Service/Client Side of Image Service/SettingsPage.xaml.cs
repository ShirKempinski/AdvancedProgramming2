using System.Windows.Controls;

namespace Client_Side_of_Image_Service
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private SettingsViewModel vm;

        public SettingsPage()
        {
            vm = new SettingsViewModel();
            DataContext = vm;
            InitializeComponent();
        }
    }
}
