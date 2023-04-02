using Orders.Controllers;
using Orders.Models;

namespace Orders.Data
{
    public interface IOrdersData
    {
        Task<IEnumerable<Order>?> GetAllOrdersAsync();
        Task<IEnumerable<Order>?> GetFilteredOrdersAsync(OrderFilterModel model);
        Task<IEnumerable<Provider>> GetAllProvidersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<bool> AddOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderItemsAsync(List<OrderItem> deleteOrderItems);
    }
}
