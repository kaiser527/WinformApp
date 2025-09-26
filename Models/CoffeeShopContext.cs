using Microsoft.EntityFrameworkCore;
using WinFormApp.Seeder;

namespace WinFormApp.Models
{
    internal class CoffeeShopContext : DbContext
    {
        public DbSet<TableFood> TableFoods { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillInfo> BillInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"
                    Data Source=MSI\SQLEXPRESS;
                    Initial Catalog=CoffeeShop;
                    Integrated Security=True;
                    Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TableFood
            modelBuilder.Entity<TableFood>()
                .HasMany(t => t.Bills)
                .WithOne(b => b.Table)
                .HasForeignKey(b => b.IdTable);

            // FoodCategory -> Food
            modelBuilder.Entity<FoodCategory>()
                .HasMany(fc => fc.Foods)
                .WithOne(f => f.Category)
                .HasForeignKey(f => f.IdCategory);

            // Bill -> BillInfo
            modelBuilder.Entity<Bill>()
                .HasMany(b => b.BillInfos)
                .WithOne(bi => bi.Bill)
                .HasForeignKey(bi => bi.IdBill);

            // Food -> BillInfo
            modelBuilder.Entity<Food>()
                .HasMany(f => f.BillInfos)
                .WithOne(bi => bi.Food)
                .HasForeignKey(bi => bi.IdFood);

            DataSeeder.Seed(modelBuilder);
        }
    }
}
