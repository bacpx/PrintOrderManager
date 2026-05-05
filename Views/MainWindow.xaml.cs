using PrintOrderManager.ViewModels;
using System.Windows;

namespace PrintOrderManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
