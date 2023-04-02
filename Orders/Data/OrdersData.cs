using Microsoft.EntityFrameworkCore;
using Orders.Controllers;
using Orders.Data.Context;
using Orders.Models;

namespace Orders.Data
{
    public class OrdersData : IOrdersData
    {
        private readonly OrderContext _orderContext;

        public OrdersData(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
        {
            return await _orderContext.ProviderList.ToListAsync();
        }

        public async Task<IEnumerable<Order>?> GetAllOrdersAsync()
        {
            return await _orderContext.OrderList
                .Include(provider => provider.Provider)
                .Include(orderItems => orderItems.OrderItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>?> GetFilteredOrdersAsync(OrderFilterModel? model)
        {
            var orderList = _orderContext.OrderList
                .Include(provider => provider.Provider)
                .Include(orderItems => orderItems.OrderItems);
            
            if (model == null)
                return await orderList.ToListAsync();

            
            var ordersQuery = _orderContext.OrderList.AsQueryable();
            
            if (model.Number is not null && model.Number.Length > 0 && !model.Number.Contains(null))
            {
                ordersQuery = ordersQuery.Where(order => model.Number.Contains(order.Number));
            }
            
            if (model.ProvidersId is not null && model.ProvidersId.Length > 0)
            {
                ordersQuery = ordersQuery.Where(order => model.ProvidersId.Contains(order.ProviderId));
            }
            
            if (model.OrderItem is not null && model.OrderItem.Length > 0 && !model.OrderItem.Contains(null))
            {
                ordersQuery = ordersQuery.Where(order =>
                    order.OrderItems.Any(item => model.OrderItem.Contains(item.Name)));
            }
            
            if (model.OrderUnit is not null && model.OrderUnit.Length > 0 && !model.OrderUnit.Contains(null))
            {
                ordersQuery = ordersQuery.Where(order =>
                    order.OrderItems.Any(item => model.OrderUnit.Contains(item.Unit)));
            }

            if (model.StartDate is not null)
            {
                ordersQuery = ordersQuery.Where(o => o.Date >= model.StartDate);
            }
            
            if (model.EndDate is not null)
            {
                ordersQuery = ordersQuery.Where(o => o.Date <= model.EndDate);
            }
            
            var orders = await ordersQuery
                .Include(provider => provider.Provider)
                .Include(orderItems => orderItems.OrderItems)
                .ToListAsync();

            return orders;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            var orders = _orderContext.OrderList
                .Include(provider => provider.Provider)
                .Include(orderItems => orderItems.OrderItems);;
            return await orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> AddOrderAsync(Order order)
        {
            _orderContext.OrderList.Add(order);
            var result = await _orderContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteOrderAsync(Order order)
        {
            _orderContext.Remove(order);
            var result = await _orderContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            _orderContext.Update(order);
            var result = await _orderContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteOrderItemsAsync(List<OrderItem> orderItems)
        {
            _orderContext.RemoveRange(orderItems);
            var result = await _orderContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
