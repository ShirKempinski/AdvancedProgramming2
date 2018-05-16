using System.Windows.Controls;


namespace Client_Side_of_Image_Service
{
    /// <summary>
    /// Interaction logic for LogPage.xaml
    /// </summary>
    public partial class LogPage : UserControl
    {
        private LogViewModel vm;
        //private static StatusToColorConverter converter;

        public LogPage()
        {
            InitializeComponent();
            vm = new LogViewModel();
            //converter = new StatusToColorConverter();
            DataContext = vm;
            foreach(LogEntry entry in vm.getEntries())
            {
                logsBox.Items.Add(entry);
            }
        }
    }
}
