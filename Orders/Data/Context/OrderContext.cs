using Microsoft.EntityFrameworkCore;
using Orders.Models;

namespace Orders.Data.Context;

public class OrderContext : DbContext
{
    public DbSet<Provider> ProviderList => Set<Provider>();
    public DbSet<Order> OrderList => Set<Order>();
    
    public DbSet<OrderItem> OrderItemList => Set<OrderItem>();
    
    public OrderContext(DbContextOptions<OrderContext> options)
        : base(options)
    
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUniques(modelBuilder);
        ConfigurePrimaryKeys(modelBuilder);
        ConfigureRelationships(modelBuilder);
        SeedData(modelBuilder);
    }

    private static void ConfigureUniques(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasIndex(o => new { o.Number, o.ProviderId })
            .IsUnique();
    }

    private void ConfigurePrimaryKeys(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .ToTable("Orders")
            .HasKey(o => o.Id);

        modelBuilder.Entity<OrderItem>()
            .ToTable("OrderItems")
            .HasKey(oi => oi.Id);

        modelBuilder.Entity<Provider>()
            .ToTable("Providers")
            .HasKey(p => p.Id);
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Provider)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.ProviderId);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var provider = new Provider() { Id = 1, Name = "ООО 'КПК'" };
        var order = new Order
        {
            Id = 1,
            Number = "ORD-123",
            Date = new DateTime(2023, 03, 02),
            ProviderId = provider.Id,
            OrderItems = new List<OrderItem>(),
        };
        var order2 = new Order
        {
            Id = 2,
            Number = "ORD-110",
            Date = new DateTime(2022, 12, 31),
            ProviderId = provider.Id,
            OrderItems = new List<OrderItem>()
        };
        var orderItem = new OrderItem
        {
            Id = 1,
            Name = "КП Бух",
            Quantity = 1,
            Unit = "шт.",
            OrderId = order.Id
        };
        var orderItem2 = new OrderItem
        {
            Id = 2,
            Name = "КП Юр",
            Quantity = 2,
            Unit = "мес.",
            OrderId = order2.Id
        };

        modelBuilder.Entity<Provider>()
            .HasData(
                provider,
                new Provider() { Id = 2, Name = "ООО 'КПВ'" },
                new Provider() { Id = 3, Name = "ООО 'КПУ'" },
                new Provider() { Id = 4, Name = "ООО 'КПП'" }
            );

        modelBuilder.Entity<OrderItem>()
            .HasData(orderItem, orderItem2);

        modelBuilder.Entity<Order>()
            .HasData(order, order2);
    }

}