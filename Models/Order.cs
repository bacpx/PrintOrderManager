using System.Collections.ObjectModel;

namespace PrintOrderManager.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Note { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        // Navigation property
        public virtual ObservableCollection<OrderItem> OrderItems { get; set; } = new ObservableCollection<OrderItem>();
    }
}
