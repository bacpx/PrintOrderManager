using Microsoft.EntityFrameworkCore;
using PrintOrderManager.Data;
using PrintOrderManager.Helpers;
using PrintOrderManager.Models;
using PrintOrderManager.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PrintOrderManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Order> _orders = new ObservableCollection<Order>();
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set { _orders = value; OnPropertyChanged(); }
        }

        private DateTime? _searchDate;
        public DateTime? SearchDate
        {
            get => _searchDate;
            set { _searchDate = value; OnPropertyChanged(); }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearSearchCommand { get; set; }
        public RelayCommand AddOrderCommand { get; set; }
        public RelayCommand EditOrderCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }
        public RelayCommand ExportExcelCommand { get; set; }
        public RelayCommand BackupDatabaseCommand { get; set; }

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(_ => LoadOrders());
            ClearSearchCommand = new RelayCommand(_ => { SearchDate = null; LoadOrders(); });
            AddOrderCommand = new RelayCommand(_ => AddOrder());
            EditOrderCommand = new RelayCommand(EditOrder, CanEditOrDelete);
            DeleteOrderCommand = new RelayCommand(DeleteOrder, CanEditOrDelete);
            ExportExcelCommand = new RelayCommand(_ => ExportExcel());
            BackupDatabaseCommand = new RelayCommand(_ => BackupDatabase());

            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var db = new AppDbContext())
            {
                var query = db.Orders.Include(o => o.OrderItems).AsQueryable();

                if (SearchDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate.Date == SearchDate.Value.Date);
                }

                Orders = new ObservableCollection<Order>(query.OrderByDescending(o => o.OrderDate).ToList());
            }
        }

        private void AddOrder()
        {
            var orderEditWindow = new OrderEditWindow();
            if (orderEditWindow.ShowDialog() == true)
            {
                LoadOrders();
            }
        }

        private void EditOrder(object? parameter)
        {
            if (parameter is Order order)
            {
                var orderEditWindow = new OrderEditWindow(order.Id);
                if (orderEditWindow.ShowDialog() == true)
                {
                    LoadOrders();
                }
            }
        }

        private bool CanEditOrDelete(object? parameter)
        {
            return parameter is Order;
        }

        private void DeleteOrder(object? parameter)
        {
            if (parameter is Order order)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa đơn hàng này không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var entity = db.Orders.Find(order.Id);
                        if (entity != null)
                        {
                            db.Orders.Remove(entity);
                            db.SaveChanges();
                            LoadOrders();
                        }
                    }
                }
            }
        }

        private void ExportExcel()
        {
            try
            {
                ExcelExportHelper.ExportOrders(Orders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackupDatabase()
        {
            try
            {
                DatabaseHelper.BackupDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi sao lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
