using Microsoft.AspNetCore.Mvc.Rendering;
using Orders.Models;

namespace Orders.ViewModels;

public class CreateOrUpdateViewModel
{
   public Order Order { get; set; } = null!;
   public int Id { get; set; }
   public List<SelectListItem> ProviderList { get; set; } = null!;

}