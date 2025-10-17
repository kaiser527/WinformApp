using Microsoft.EntityFrameworkCore;

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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

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
            // TableFood -> Bill
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

            // Role <-> Permission 
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // Account -> Role
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.IdRole);
        }
    }
}
