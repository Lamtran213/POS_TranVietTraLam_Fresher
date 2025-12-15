using Microsoft.EntityFrameworkCore;
using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Context
{
    public class POSContext : DbContext
    {
        public POSContext(DbContextOptions<POSContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== USER =====
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            // ===== CART =====
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== CART ITEM =====
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== CATEGORY =====
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            // ===== PRODUCT =====
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== ORDER =====
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Users)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasConversion<int>(); // enum -> int

            // ===== ORDER DETAIL =====
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= OTP =================
            modelBuilder.Entity<OTP>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<OTP>()
                .Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<OTP>()
                .Property(o => o.OTPCode)
                .IsRequired()
                .HasMaxLength(6);

            modelBuilder.Entity<OTP>()
                .Property(o => o.Purpose)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<OTP>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("NOW()");

            // Index cho verify nhanh
            modelBuilder.Entity<OTP>()
                .HasIndex(o => new { o.Email, o.Purpose });

            // Unique OTP đang active (IsUsed = false)
            modelBuilder.Entity<OTP>()
                .HasIndex(o => new { o.Email, o.Purpose })
                .IsUnique()
                .HasFilter("\"IsUsed\" = FALSE");
        }
    }
}
