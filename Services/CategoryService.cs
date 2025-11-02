using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<FoodCategory>> GetListCategory(string name = null)
        {
            using (var context = new CoffeeShopContext())
            {
                var query = context.FoodCategories.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(c => c.Name.ToLower().Contains(name.ToLower()));
                }

                return await query.ToListAsync();
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
