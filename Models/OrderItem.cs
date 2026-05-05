using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrintOrderManager.Models
{
    public class OrderItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        
        private string _productName = string.Empty;
        public string ProductName 
        { 
            get => _productName; 
            set { _productName = value; OnPropertyChanged(); } 
        }

        private string _size = string.Empty;
        public string Size 
        { 
            get => _size; 
            set { _size = value; OnPropertyChanged(); } 
        }

        public int? MaterialId { get; set; }
        private Material? _material;
        public virtual Material? Material 
        { 
            get => _material; 
            set 
            { 
                _material = value; 
                MaterialId = value?.Id;
                OnPropertyChanged(); 
            } 
        }

        public int? ProcessId { get; set; }
        private Process? _process;
        public virtual Process? Process 
        { 
            get => _process; 
            set 
            { 
                _process = value; 
                ProcessId = value?.Id;
                OnPropertyChanged(); 
            } 
        }

        private int _quantity;
        public int Quantity 
        { 
            get => _quantity; 
            set { _quantity = value; CalculateAmount(); OnPropertyChanged(); } 
        }

        private decimal _unitPrice;
        public decimal UnitPrice 
        { 
            get => _unitPrice; 
            set { _unitPrice = value; CalculateAmount(); OnPropertyChanged(); } 
        }

        private decimal _amount;
        public decimal Amount 
        { 
            get => _amount; 
            private set { _amount = value; OnPropertyChanged(); } 
        }

        public virtual Order? Order { get; set; }

        private void CalculateAmount()
        {
            Amount = Quantity * UnitPrice;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
