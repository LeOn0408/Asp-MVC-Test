namespace Orders.Models;

public class OrderFilterModel
{
    public string[]? Number { get; set; }
    
    public int[]? ProvidersId  { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string[]? OrderItem  { get; set; }
    public string[]? OrderUnit  { get; set; }
}