using Orders.Models;

namespace Orders.ViewModels;

public class OrderFiltersViewModel
{
    public List<string> NumberFilter { get; set; }
    public List<string> OrderItemNameFilter { get; set; }
    public List<string> OrderItemUnitFilter { get; set; }
    public List<Provider?> ProviderFilter { get; set; }
    public List<Order>? Orders { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
    public DateTime EndDate { get; set; } = DateTime.Now;
}