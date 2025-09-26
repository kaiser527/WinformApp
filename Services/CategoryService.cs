using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<FoodCategory>> GetListCategory()
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.FoodCategories.ToListAsync();
            }
        }
    }
}
