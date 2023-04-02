using Microsoft.AspNetCore.Mvc;
using Orders.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orders.Data;
using Orders.ViewModels;

namespace Orders.Controllers;

public class OrderController : Controller
{ 
    private readonly ILogger<OrderController> _logger;
    private readonly IOrdersData _orders;

    public OrderController(ILogger<OrderController> logger, IOrdersData orders)
    {
        _logger = logger;
        _orders = orders;
    }
    
    
    /// <summary>
    /// Обработка индексной страницы
    /// </summary>
    /// <param name="filter">Данные фильтра</param>
    /// <returns>Индексную страницу</returns>
    public async Task<IActionResult> Index(OrderFilterModel filter)
    {
        var allOrders = await _orders.GetAllOrdersAsync();
        
            
        if (allOrders == null) 
            return View(new OrderFiltersViewModel());

        var enumerable = allOrders.ToList();
        var orderItemFilter = enumerable.SelectMany(order => order.OrderItems).Distinct().ToList();
        var orders = await _orders.GetFilteredOrdersAsync(filter);
        var orderFilters = new OrderFiltersViewModel
        {
            NumberFilter = enumerable.Select(order => order.Number).Distinct().ToList(),
            ProviderFilter = enumerable.Select(order => order.Provider).Distinct().ToList(),
            OrderItemNameFilter = orderItemFilter.Select(item => item.Name).Distinct().ToList(),
            OrderItemUnitFilter = orderItemFilter.Select(item => item.Unit).Distinct().ToList(),
            Orders = orders?.ToList()
        };
        return View(orderFilters);
    }
     
    /// <summary>
    /// Просмотр заказа
    /// </summary>
    /// <param name="id">Ид заказа</param>
    /// <returns>Страница с информацией о заказе</returns>
    public async Task<IActionResult> ViewOrder(int id)
    {
        Order? order = await _orders.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }
    
    /// <summary>
    /// даляет заказ
    /// </summary>
    /// <param name="id">Ид заказа</param>
    /// <returns>Возвращает на страницу заказов</returns>
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _orders.GetOrderByIdAsync(id);
        
        if (order == null)
        {
            return NotFound();
        }
        if (await _orders.DeleteOrderAsync(order)) 
            return RedirectToAction("Index", this);
            
        ModelState.AddModelError(string.Empty,"Ошибка при удалении");
        return View("ViewOrder", order);
    }
    
    /// <summary>
    /// Создает страницу с новым заказом в режиме редактирования
    /// </summary>
    /// <returns>Страница редактирования заказа</returns>
    public async Task<IActionResult> CreateOrder()
    {
        var order = new Order()
        {
            Date = DateTime.Today,
        };
            
        var viewModel = new CreateOrUpdateViewModel
        {
            Order = order,
            Id = order.Id,
            ProviderList = await GetSelectListProviders()
        };
        return View("CreateOrUpdate", viewModel);
    }
     
    /// <summary>
    /// Создает страницу редактирования заказа
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Страница редактирования заказа</returns>
    public async Task<IActionResult> EditOrder(int id)
    {
        var order = await _orders.GetOrderByIdAsync(id);
        
        if (order == null)
        {
            return NotFound();
        }
        
        var viewModel = new CreateOrUpdateViewModel
        {
            Order = order,
            Id = order.Id,
            ProviderList = await GetSelectListProviders()
        };
        return View("CreateOrUpdate", viewModel);
    }
     
    /// <summary>
    /// Обработчик сохранения заказа
    /// </summary>
    /// <param name="order">Заказ</param>
    [HttpPost]
    public async Task<IActionResult> SaveOrder(Order order)
    {
        if (ModelState.IsValid)
        {
            if (order.Id == 0)
            {
                if (await HandleNewOrder(order)) 
                    return RedirectToAction("Index");
            }
            else
            {
                if (await HandleExistingOrder(order))
                    return RedirectToAction("Index");
            }
        }
        var viewModel = new CreateOrUpdateViewModel
        {
            Order = order,
            Id = order.Id,
            ProviderList = await GetSelectListProviders()
        };
        
        return View("CreateOrUpdate", viewModel);
    }
      
    /// <summary>
    /// Обработка измененного заказа
    /// </summary>
    /// <param name="order">Заказ</param>
    /// <returns>True если изменения успешны, False если нет</returns>
    private async Task<bool> HandleExistingOrder(Order order)
    {
        var duplicateOrders = await GetDuplicateOrders(order);
        if (duplicateOrders.Count > 0)
        {
            ModelState.AddModelError(string.Empty,
                "Заказ с таким номером уже существует для данного поставщика");
        }
        else
        {
            var originalOrder = await _orders.GetOrderByIdAsync(order.Id);
            if (originalOrder == null)
                ModelState.AddModelError(string.Empty,
                    "Заказ с таким номером уже существует для данного поставщика");
            else
            {
                await DeleteMissingOrderItemsAsync(order, originalOrder);

                AddNewOrderItems(order, originalOrder);
                
                originalOrder.Number = order.Number;
                originalOrder.Date = order.Date;
                originalOrder.ProviderId = order.ProviderId;
        
                if (await _orders.UpdateOrderAsync(originalOrder))
                    return true;
                ModelState.AddModelError(string.Empty, "Ошибка при изменении");
            }
        }
        
        return false;
    }

   /// <summary>
   /// Возвращает список дублей у заказа по Номеру и Поставщику
   /// </summary>
   /// <param name="order">Заказ</param>
   /// <returns>Список дублей</returns>
    private async Task<List<Order>> GetDuplicateOrders(Order order)
    {
        var duplicateOrders = await _orders.GetFilteredOrdersAsync(new OrderFilterModel()
        {
            Number = new[] { order.Number },
            ProvidersId = new[] { order.ProviderId }
        });
        return duplicateOrders?.ToList() ?? new List<Order>();
    }

   /// <summary>
   /// Добавляет новые элементы заказа в заказ
   /// </summary>
   /// <param name="order">заказ</param>
   /// <param name="originalOrder">отредактированный заказ</param>
    private void AddNewOrderItems(Order order, Order originalOrder)
    {
        var addOrderItems = order.OrderItems.Where(orderItem =>
                originalOrder.OrderItems.FirstOrDefault(item => item.Id == orderItem.Id) is null)?
            .ToList();
        if (addOrderItems is not null && addOrderItems.Count > 0)
        {
            originalOrder.OrderItems.AddRange(addOrderItems);
        }
    }

   /// <summary>
   /// Удаляет элементы заказа из заказа
   /// </summary>
   /// <param name="order">заказ</param>
   /// <param name="originalOrder">отредактированный заказ</param>
    private async Task DeleteMissingOrderItemsAsync(Order order, Order originalOrder)
    {
        var deleteOrderItems = originalOrder.OrderItems.Where(orderItem =>
                order.OrderItems.FirstOrDefault(item => item.Id == orderItem.Id) is null)?
            .ToList();
        if (deleteOrderItems is not null && deleteOrderItems.Count > 0)
        {
            await _orders.DeleteOrderItemsAsync(deleteOrderItems);
        }
    }

    /// <summary>
    /// Обработка добавленного заказа
    /// </summary>
    /// <param name="order">Заказ</param>
    /// <returns>True - успешная обработка, False - Ошибка</returns>
    private async Task<bool> HandleNewOrder(Order order)
    {
        var duplicateOrder = await GetDuplicateOrders(order);
        if (duplicateOrder.Count > 0)
        {
            ModelState.AddModelError(string.Empty,
                "Заказ с таким номером уже существует для данного поставщика");
        }
        else
        {
            if (await _orders.AddOrderAsync(order))
                return true;
        
            ModelState.AddModelError(string.Empty, "Ошибка при добавлении");
        }
        
        return false;
    }
    
    /// <summary>
    /// Формирует список поставщиков
    /// </summary>
    /// <returns>Список поставщиков</returns>
    private async Task<List<SelectListItem>> GetSelectListProviders()
    {
        var providers = await _orders.GetAllProvidersAsync();
        var providerList = providers.Select(p => new SelectListItem() { Value = p.Id.ToString(), Text = p.Name }).ToList();
        return providerList;
    }
}