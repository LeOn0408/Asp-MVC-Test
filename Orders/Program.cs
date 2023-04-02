using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Data.Context;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

string? connectionString = configuration.GetConnectionString("Orders");
builder.Services.AddDbContext<OrderContext>(options => options.UseSqlite(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IOrdersData, OrdersData>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<OrderContext>();
    context?.Database.EnsureCreated();
}

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "vieworder",
        pattern: "order/view/{id}",
        defaults: new { controller = "Order", action = "ViewOrder" });

    endpoints.MapControllerRoute(
        name: "orderlist",
        pattern: "order/list",
        defaults: new { controller = "Order", action = "Index" });
    
    //Откроем список по умолчанию
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Order}/{action=Index}/{id?}");
});

app.Run();
