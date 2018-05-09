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
            InitializeComponent();
            vm = new LogViewModel();
            DataContext = vm;
            for (int i = 0; i < vm.logs.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = vm.logs[i];
                listBox.Items.Add(item);
            }
        }

    }
}
