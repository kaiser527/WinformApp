using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class FoodService
    {
        private static FoodService instance;

        public static FoodService Instance
        {
            get
            {
                if (instance == null) instance = new FoodService();
                return FoodService.instance;
            }
            private set { FoodService.instance = value; }
        }

        public async Task<IEnumerable<Food>> GetListFoodByCategoryID(int id)
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.Foods.Where(f => f.IdCategory == id).ToListAsync();
            }
        }
    }
}
