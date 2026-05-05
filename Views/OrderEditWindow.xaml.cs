using PrintOrderManager.ViewModels;
using System.Windows;

namespace PrintOrderManager.Views
{
    public partial class OrderEditWindow : Window
    {
        public OrderEditWindow(int? orderId = null)
        {
            InitializeComponent();
            var vm = new OrderEditViewModel(orderId);
            vm.CloseAction = () =>
            {
                DialogResult = true;
                Close();
            };
            DataContext = vm;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
