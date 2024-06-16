using Microsoft.EntityFrameworkCore;
using SmallBusiness.Models;
using System;

namespace SmallBusiness.DB_context
{
    public class StorDbContext : DbContext
    {
        public StorDbContext(DbContextOptions<StorDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {




            modelBuilder.Entity<User>()
             .HasOne(p => p.Cart)
             .WithOne(d => d.User)
             .HasForeignKey<Cart>(d => d.UserID);



            modelBuilder.Entity<Review>()
           .HasOne(r => r.User)
           .WithMany(u => u.Reviews)
           .HasForeignKey(r => r.UserId)
           .OnDelete(DeleteBehavior.Restrict);


            //esraa

            modelBuilder.Entity<OrderItem>()
                .HasKey(o => new { o.OrderID, o.ItemID });

            modelBuilder.Entity<OrderItem>()
                .HasOne<Order>(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne<Item>(i => i.Item)
                .WithMany(i => i.ItemOrders)
                .HasForeignKey(i => i.ItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // last



        }






        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Catagery> Catagerys { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Brand> Brands { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}
