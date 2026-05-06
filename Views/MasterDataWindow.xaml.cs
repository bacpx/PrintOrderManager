using PrintOrderManager.ViewModels;
using System.Windows;

namespace PrintOrderManager.Views
{
    public partial class MasterDataWindow : Window
    {
        public MasterDataWindow()
        {
            InitializeComponent();
            var vm = new MasterDataViewModel();
            vm.CloseAction = () => this.Close();
            this.DataContext = vm;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.DataContext is MasterDataViewModel vm)
            {
                vm.Dispose();
            }
            base.OnClosed(e);
        }
    }
}
