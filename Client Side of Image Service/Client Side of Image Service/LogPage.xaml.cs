using System.Windows.Controls;


namespace Client_Side_of_Image_Service
{
    /// <summary>
    /// Interaction logic for LogPage.xaml
    /// </summary>
    public partial class LogPage : UserControl
    {
        private LogViewModel vm;

        public LogPage()
        {
            vm = new LogViewModel();
            DataContext = vm;
            InitializeComponent();
        }
    }
}
