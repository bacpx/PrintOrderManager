using Microsoft.EntityFrameworkCore;
using PrintOrderManager.Models;
using System.IO;

namespace PrintOrderManager.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<Process> Processes { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            // Optional: Enable sensitive data logging for debugging
            // optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Relationships
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Material)
                .WithMany()
                .HasForeignKey(oi => oi.MaterialId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Process)
                .WithMany()
                .HasForeignKey(oi => oi.ProcessId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed Data
            modelBuilder.Entity<Material>().HasData(
                new Material { Id = 1, Name = "Đề can nhựa" },
                new Material { Id = 2, Name = "OP120" },
                new Material { Id = 3, Name = "OP140" }
            );

            modelBuilder.Entity<Process>().HasData(
                new Process { Id = 1, Name = "Cán bóng" },
                new Process { Id = 2, Name = "Bế TP" },
                new Process { Id = 3, Name = "Cán bóng bế TP" },
                new Process { Id = 4, Name = "Khuôn mới" }
            );
        }
    }
}
