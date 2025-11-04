using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.DTO;
using WinFormApp.Forms;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class CategoryService
    {
        private static CategoryService instance;

        public static CategoryService Instance
        {
            get
            {
                if (instance == null) instance = new CategoryService();
                return CategoryService.instance;
            }
            private set { CategoryService.instance = value; }
        }

        public async Task<PaginatedResult<FoodCategory>> GetListCategory(
            int pageSize = 100,
            int pageNumber = 1,
            string name = null)
        {
            using (var context = new CoffeeShopContext())
            {
                var query = context.FoodCategories.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(c => c.Name.ToLower().Contains(name.ToLower()));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var items = await query
                    .OrderBy(f => f.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginatedResult<FoodCategory>
                {
                    Items = items,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };
            }
        }

        public async Task InsertCategory(FoodCategory category)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isExist = await context.TableFoods.AnyAsync(c => c.Name == category.Name);

                if (isExist)
                {
                    Alert.ShowAlert("Category is already exist", Alert.AlertType.Error);
                    return;
                }

                context.FoodCategories.Add(category);

                await context.SaveChangesAsync();   
            }
        }

        public async Task UpdateCategory(FoodCategory category)
        {
            using (var context = new CoffeeShopContext())
            {
                FoodCategory updatedCategory = await context.FoodCategories.FindAsync(category.Id);

                if (updatedCategory == null)
                {
                    Alert.ShowAlert("Category is not exist", Alert.AlertType.Error);
                    return;
                }

                bool isExist = await context.FoodCategories
                    .AnyAsync(c => c.Name == category.Name && c.Id != c.Id);

                if (isExist)
                {
                    Alert.ShowAlert("Category is already exist", Alert.AlertType.Error);
                    return;
                }

                updatedCategory.Name = category.Name;

                await context.SaveChangesAsync();   
            }
        }

        public async Task DeleteCategory(int categoryId)
        {
            using (var context = new CoffeeShopContext())
            {
                FoodCategory category = await context.FoodCategories
                    .Include(f => f.Foods)
                    .FirstOrDefaultAsync(f => f.Id == categoryId);

                if (category == null)
                {
                    Alert.ShowAlert("Category is not exist", Alert.AlertType.Error);
                    return;
                }

                if (category.Foods.Any())
                {
                    Alert.ShowAlert("There are some food related to this category", Alert.AlertType.Error);
                    return;
                }

                context.FoodCategories.Remove(category);

                await context.SaveChangesAsync();   
            }
        }
    }
}
