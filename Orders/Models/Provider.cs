namespace Orders.Models
{
    public class Provider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
