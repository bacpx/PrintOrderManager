using Microsoft.EntityFrameworkCore;
using PrintOrderManager.Data;
using PrintOrderManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PrintOrderManager.ViewModels
{
    public class OrderEditViewModel : INotifyPropertyChanged
    {
        private readonly int? _orderId;
        private Order _currentOrder = new Order();

        public Order CurrentOrder
        {
            get => _currentOrder;
            set { _currentOrder = value; OnPropertyChanged(); }
        }

        private ObservableCollection<OrderItem> _items = new ObservableCollection<OrderItem>();
        public ObservableCollection<OrderItem> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Material> _materials = new ObservableCollection<Material>();
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set { _materials = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Process> _processes = new ObservableCollection<Process>();
        public ObservableCollection<Process> Processes
        {
            get => _processes;
            set { _processes = value; OnPropertyChanged(); }
        }

        public RelayCommand AddItemCommand { get; set; }
        public RelayCommand RemoveItemCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }

        public Action? CloseAction { get; set; }

        public OrderEditViewModel(int? orderId = null)
        {
            _orderId = orderId;

            AddItemCommand = new RelayCommand(_ => AddItem());
            RemoveItemCommand = new RelayCommand(RemoveItem, CanRemoveItem);
            SaveCommand = new RelayCommand(_ => Save());

            Items.CollectionChanged += (s, e) => CalculateTotalAmount();
            
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                Materials = new ObservableCollection<Material>(db.Materials.ToList());
                Processes = new ObservableCollection<Process>(db.Processes.ToList());

                if (_orderId.HasValue)
                {
                    var order = db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == _orderId.Value);
                    if (order != null)
                    {
                        CurrentOrder = order;
                        Items = new ObservableCollection<OrderItem>(order.OrderItems);
                        foreach (var item in Items)
                        {
                            item.PropertyChanged += Item_PropertyChanged;
                        }
                    }
                }
                else
                {
                    CurrentOrder = new Order();
                }
            }
        }

        private void AddItem()
        {
            var newItem = new OrderItem();
            newItem.PropertyChanged += Item_PropertyChanged;
            Items.Add(newItem);
        }

        private bool CanRemoveItem(object? parameter)
        {
            return parameter is OrderItem;
        }

        private void RemoveItem(object? parameter)
        {
            if (parameter is OrderItem item)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                Items.Remove(item);
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderItem.Amount))
            {
                CalculateTotalAmount();
            }
        }

        private void CalculateTotalAmount()
        {
            CurrentOrder.TotalAmount = Items.Sum(i => i.Amount);
            OnPropertyChanged(nameof(CurrentOrder));
        }

        private void Save()
        {
            if (Items.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất 1 sản phẩm vào đơn hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Items.Any(i => i.Quantity <= 0))
            {
                MessageBox.Show("Số lượng sản phẩm phải lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (_orderId.HasValue)
                {
                    var existingOrder = db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == _orderId.Value);
                    if (existingOrder != null)
                    {
                        existingOrder.OrderDate = CurrentOrder.OrderDate;
                        existingOrder.Note = CurrentOrder.Note;
                        existingOrder.TotalAmount = CurrentOrder.TotalAmount;

                        db.OrderItems.RemoveRange(existingOrder.OrderItems);
                        foreach (var item in Items)
                        {
                            item.Id = 0; // Reset Id for EF to track as new
                            existingOrder.OrderItems.Add(item);
                        }
                    }
                }
                else
                {
                    CurrentOrder.OrderItems = Items;
                    db.Orders.Add(CurrentOrder);
                }

                db.SaveChanges();
                CloseAction?.Invoke();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
