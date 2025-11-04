using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.DTO;
using WinFormApp.Forms;
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

        public async Task<PaginatedResult<FoodDTO>> GetListFood(
            int pageSize = 100,
            int pageNumber = 1,
            string name = null)
        {
            using (var context = new CoffeeShopContext())
            {
                var query = context.Foods
                    .Include(f => f.Category)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(f => 
                        f.Name.ToLower().Contains(name.ToLower()) ||
                        f.Category.Name.ToLower().Contains(name.ToLower())
                    );
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var items = await query
                 .OrderBy(f => f.Id)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .Select(f => new FoodDTO(
                        f.Id,
                        f.Name,
                        f.Category.Name,
                        (float)f.Price
                 ))
                 .ToListAsync();

                return new PaginatedResult<FoodDTO>
                {
                    Items = items,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };
            }
        }

        public async Task InsertFood(FoodDTO foodDTO)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isExist = await context.Foods.AnyAsync(f => f.Name == foodDTO.Name);

                if (isExist)
                {
                    Alert.ShowAlert("Food is already exist", Alert.AlertType.Error);
                    return;
                }

                FoodCategory category = await context.FoodCategories.FirstOrDefaultAsync(c => c.Name == foodDTO.Category);

                if(category == null)
                {
                    Alert.ShowAlert("Category is not exist", Alert.AlertType.Error);
                    return;
                }

                Food food = new Food()
                {
                    Name = foodDTO.Name,
                    IdCategory = category.Id,
                    Price = foodDTO.Price
                };

                context.Foods.Add(food);

                await context.SaveChangesAsync();   
            }
        }

        public async Task UpdateFood(FoodDTO foodDTO) 
        {
            using (var context = new CoffeeShopContext())
            {
                Food food = await context.Foods.FindAsync(foodDTO.Id);

                if (food == null) 
                {
                    Alert.ShowAlert("Food is not exist", Alert.AlertType.Error);
                    return;
                }

                bool isExist = await context.Foods.AnyAsync(f => f.Name == foodDTO.Name && f.Id != foodDTO.Id);

                if (isExist)
                {
                    Alert.ShowAlert("Food is already exist", Alert.AlertType.Error);
                    return;
                }

                FoodCategory category = await context.FoodCategories.FirstOrDefaultAsync(c => c.Name == foodDTO.Category);

                if (category == null)
                {
                    Alert.ShowAlert("Category is not exist", Alert.AlertType.Error);
                    return;
                }

                food.Name = foodDTO.Name;
                food.IdCategory = category.Id;
                food.Price = foodDTO.Price;

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<TableFood>> DeleteFood(int foodId)
        {
            using (var context = new CoffeeShopContext())
            {
                Food food = await context.Foods
                    .Include(f => f.BillInfos)
                        .ThenInclude(bi => bi.Bill)
                            .ThenInclude(b => b.Table)
                    .FirstOrDefaultAsync(f => f.Id == foodId);

                if (food == null)
                {
                    Alert.ShowAlert("Food is not exist", Alert.AlertType.Error);
                    return new List<TableFood>();
                }

                var relatedTables = food.BillInfos
                    .Where(bi => bi.Bill?.Table != null)
                    .Select(bi => bi.Bill.Table)
                    .Distinct()
                    .ToList();

                context.BillInfos.RemoveRange(food.BillInfos);
                context.Foods.Remove(food);

                await context.SaveChangesAsync();

                foreach (var table in relatedTables)
                {
                    await context.Entry(table)
                        .Collection(t => t.Bills)
                        .Query()
                        .Include(b => b.BillInfos)
                        .LoadAsync();

                    bool isEmpty = table.Bills
                        .Where(b => b.Status == 0)
                        .All(b => !b.BillInfos.Any());

                    if (isEmpty)
                    {
                        table.Status = "Empty";
                    }
                }

                await context.SaveChangesAsync();

                return relatedTables;
            }
        }
    }
}
