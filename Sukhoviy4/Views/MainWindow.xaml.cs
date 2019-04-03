using System.Windows;


namespace Sukhoviy4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new UserListViewModel(delegate () { Dispatcher.Invoke(UsersDataGrid.Items.Refresh); });
        }
    }
}
