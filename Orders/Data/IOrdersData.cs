using Orders.Controllers;
using Orders.Models;

namespace Orders.Data
{
    /// <summary>
    /// Интерфейс для работы с данными заказов
    /// </summary>
    public interface IOrdersData
    {
        /// <summary>
        /// Получить все заказы
        /// </summary>
        /// <returns>Список заказов</returns>
        Task<IEnumerable<Order>?> GetAllOrdersAsync();
        /// <summary>
        /// Получить отфильтрованных заказов
        /// </summary>
        /// <param name="model">Данные для фильтрации</param>
        /// <returns>Отфильтрованный список заказов</returns>
        Task<IEnumerable<Order>?> GetFilteredOrdersAsync(OrderFilterModel model);
        /// <summary>
        /// Получить всех поставщиков
        /// </summary>
        /// <returns>Список поставщиков</returns>
        Task<IEnumerable<Provider>> GetAllProvidersAsync();
        /// <summary>
        /// Получить информацию по заказу
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <returns>Информация о заказе</returns>
        Task<Order?> GetOrderByIdAsync(int id);
        /// <summary>
        /// Добавить заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Результат выполнения:успешно/неуспешно</returns>
        Task<bool> AddOrderAsync(Order order);
        /// <summary>
        /// Удалить заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Результат выполнения:успешно/неуспешно</returns>
        Task<bool> DeleteOrderAsync(Order order);
        /// <summary>
        /// Обновить заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Результат выполнения:успешно/неуспешно</returns>
        Task<bool> UpdateOrderAsync(Order order);
        /// <summary>
        /// Удалить позиции заказа
        /// </summary>
        /// <param name="deleteOrderItems">Список позиций заказа</param>
        /// <returns>Результат выполнения:успешно/неуспешно</returns>
        Task<bool> DeleteOrderItemsAsync(List<OrderItem> deleteOrderItems);
    }
}
