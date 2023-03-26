using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Index("IX_Order_Number_ProviderId", 1, IsUnique = true)]
        public string Number { get; set; }
        public DateTime Date { get; set; }
        [Index("IX_Order_Number_ProviderId", 2, IsUnique = true)]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
