using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WinFormApp.Models;

namespace WinFormApp.Seeder
{
    internal class DataSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            //Seed Permission
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Create Account", Module = "Account" },
                new Permission { Id = 2, Name = "Update Account", Module = "Account" },
                new Permission { Id = 3, Name = "Delete Account", Module = "Account" },
                new Permission { Id = 4, Name = "View Account", Module = "Account" },
                new Permission { Id = 5, Name = "Create Food", Module = "Food" },
                new Permission { Id = 6, Name = "Update Food", Module = "Food" },
                new Permission { Id = 7, Name = "Delete Food", Module = "Food" },
                new Permission { Id = 8, Name = "View Food", Module = "Food" },
                new Permission { Id = 9, Name = "Create Category", Module = "Category" },
                new Permission { Id = 10, Name = "Update Category", Module = "Category" },
                new Permission { Id = 11, Name = "Delete Category", Module = "Category" },
                new Permission { Id = 12, Name = "View Category", Module = "Category" },
                new Permission { Id = 13, Name = "Create Table", Module = "Table" },
                new Permission { Id = 14, Name = "Update Table", Module = "Table" },
                new Permission { Id = 15, Name = "Delete Table", Module = "Table" },
                new Permission { Id = 16, Name = "View Table", Module = "Table" },
                new Permission { Id = 17, Name = "Create Role", Module = "Role" },
                new Permission { Id = 18, Name = "Update Role", Module = "Role" },
                new Permission { Id = 19, Name = "Delete Role", Module = "Role" },
                new Permission { Id = 20, Name = "View Role", Module = "Role" },
                new Permission { Id = 21, Name = "Create Permission", Module = "Permission" },
                new Permission { Id = 22, Name = "Update Permission", Module = "Permission" },
                new Permission { Id = 23, Name = "Delete Permission", Module = "Permission" },
                new Permission { Id = 24, Name = "View Permission", Module = "Permission" },
                new Permission { Id = 25, Name = "View Bill", Module = "Bill" }
            );

            //Seed Role
            modelBuilder.Entity<Role>().HasData(
               new Role { Id = 1, Name = "Admin", IsActive = true },
               new Role { Id = 2, Name = "Staff", IsActive = true },
               new Role { Id = 3, Name = "Tester", IsActive = true }
            );

            //Seed RolePermission
            var rolePermissions = new List<RolePermission>();

            for (int pid = 1; pid <= 24; pid++)
            {
                rolePermissions.Add(new RolePermission { RoleId = 1, PermissionId = pid });
            }

            for (int pid = 1; pid <= 16; pid++)
            {
                rolePermissions.Add(new RolePermission { RoleId = 3, PermissionId = pid });
            }

            rolePermissions.AddRange(new[]
            {
                new RolePermission { RoleId = 2, PermissionId = 16 },
                new RolePermission { RoleId = 1, PermissionId = 25 },
                new RolePermission { RoleId = 3, PermissionId = 25 }
            });

            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

            // Seed Accounts
            modelBuilder.Entity<Account>().HasData(
                new Account { UserName = "admin", DisplayName = "Administrator", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 1 },
                new Account { UserName = "john_doe", DisplayName = "John Doe", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 2 },
                new Account { UserName = "jane_smith", DisplayName = "Jane Smith", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 2 },
                new Account { UserName = "michael", DisplayName = "Michael Johnson", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 2 },
                new Account { UserName = "emily", DisplayName = "Emily Davis", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 3 },
                new Account { UserName = "david", DisplayName = "David Wilson", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 2 },
                new Account { UserName = "sarah", DisplayName = "Sarah Brown", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 3 },
                new Account { UserName = "chris", DisplayName = "Chris Lee", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 2 },
                new Account { UserName = "amanda", DisplayName = "Amanda Miller", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 2 },
                new Account { UserName = "matthew", DisplayName = "Matthew Garcia", PassWord = BCrypt.Net.BCrypt.HashPassword("123456"), IdRole = 3 }
            );

            // Seed TableFood
            modelBuilder.Entity<TableFood>().HasData(
                new TableFood { Id = 1, Name = "Table 1", Status = "Empty" },
                new TableFood { Id = 2, Name = "Table 2", Status = "Empty" },
                new TableFood { Id = 3, Name = "Table 3", Status = "Empty" },
                new TableFood { Id = 4, Name = "Table 4", Status = "Empty" },
                new TableFood { Id = 5, Name = "Table 5", Status = "Empty" },
                new TableFood { Id = 6, Name = "Table 6", Status = "Empty" },
                new TableFood { Id = 7, Name = "Table 7", Status = "Empty" },
                new TableFood { Id = 8, Name = "Table 8", Status = "Empty" },
                new TableFood { Id = 9, Name = "Table 9", Status = "Empty" },
                new TableFood { Id = 10, Name = "Table 10", Status = "Empty" },
                new TableFood { Id = 11, Name = "Table 11", Status = "Empty" },
                new TableFood { Id = 12, Name = "Table 12", Status = "Empty" },
                new TableFood { Id = 13, Name = "Table 13", Status = "Empty" },
                new TableFood { Id = 14, Name = "Table 14", Status = "Empty" },
                new TableFood { Id = 15, Name = "Table 15", Status = "Empty" },
                new TableFood { Id = 16, Name = "Table 16", Status = "Empty" },
                new TableFood { Id = 17, Name = "Table 17", Status = "Empty" },
                new TableFood { Id = 18, Name = "Table 18", Status = "Empty" },
                new TableFood { Id = 19, Name = "Table 19", Status = "Empty" },
                new TableFood { Id = 20, Name = "Table 20", Status = "Empty" });

            // Seed FoodCategory
            modelBuilder.Entity<FoodCategory>().HasData(
                new FoodCategory { Id = 1, Name = "Coffee" },
                new FoodCategory { Id = 2, Name = "Tea" },
                new FoodCategory { Id = 3, Name = "Smoothies" },
                new FoodCategory { Id = 4, Name = "Pastries" },
                new FoodCategory { Id = 5, Name = "Sandwiches" },
                new FoodCategory { Id = 6, Name = "Juices" },
                new FoodCategory { Id = 7, Name = "Ice Cream" },
                new FoodCategory { Id = 8, Name = "Specials" });

            // Seed Foods
            modelBuilder.Entity<Food>().HasData(
                new Food { Id = 1, Name = "Espresso", IdCategory = 1, Price = 2.5 },
                new Food { Id = 2, Name = "Latte", IdCategory = 1, Price = 3.5 },
                new Food { Id = 3, Name = "Cappuccino", IdCategory = 1, Price = 3.8 },
                new Food { Id = 4, Name = "Americano", IdCategory = 1, Price = 2.8 },
                new Food { Id = 5, Name = "Green Tea", IdCategory = 2, Price = 2.0 },
                new Food { Id = 6, Name = "Black Tea", IdCategory = 2, Price = 2.0 },
                new Food { Id = 7, Name = "Herbal Tea", IdCategory = 2, Price = 2.5 },
                new Food { Id = 8, Name = "Milk Tea", IdCategory = 2, Price = 3.0 },
                new Food { Id = 9, Name = "Mango Smoothie", IdCategory = 3, Price = 4.5 },
                new Food { Id = 10, Name = "Strawberry Smoothie", IdCategory = 3, Price = 4.5 },
                new Food { Id = 11, Name = "Banana Smoothie", IdCategory = 3, Price = 4.0 },
                new Food { Id = 12, Name = "Avocado Smoothie", IdCategory = 3, Price = 5.0 },
                new Food { Id = 13, Name = "Croissant", IdCategory = 4, Price = 2.5 },
                new Food { Id = 14, Name = "Muffin", IdCategory = 4, Price = 2.0 },
                new Food { Id = 15, Name = "Cheesecake", IdCategory = 4, Price = 3.5 },
                new Food { Id = 16, Name = "Chocolate Cake", IdCategory = 4, Price = 3.8 },
                new Food { Id = 17, Name = "Ham Sandwich", IdCategory = 5, Price = 4.0 },
                new Food { Id = 18, Name = "Chicken Sandwich", IdCategory = 5, Price = 4.5 },
                new Food { Id = 19, Name = "Veggie Sandwich", IdCategory = 5, Price = 3.8 },
                new Food { Id = 20, Name = "Orange Juice", IdCategory = 6, Price = 3.0 },
                new Food { Id = 21, Name = "Apple Juice", IdCategory = 6, Price = 3.0 },
                new Food { Id = 22, Name = "Carrot Juice", IdCategory = 6, Price = 3.2 },
                new Food { Id = 23, Name = "Vanilla Ice Cream", IdCategory = 7, Price = 2.5 },
                new Food { Id = 24, Name = "Chocolate Ice Cream", IdCategory = 7, Price = 2.5 },
                new Food { Id = 25, Name = "Strawberry Ice Cream", IdCategory = 7, Price = 2.5 },
                new Food { Id = 26, Name = "Affogato", IdCategory = 8, Price = 5.5 },
                new Food { Id = 27, Name = "Irish Coffee", IdCategory = 8, Price = 6.0 });
        }
    }
}
