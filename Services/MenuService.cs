using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.DTO;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class MenuService
    {
        private static MenuService instance;

        public static MenuService Instance
        {
            get
            {
                if (instance == null) instance = new MenuService();
                return MenuService.instance;
            }
            private set { MenuService.instance = value; }
        }

        public async Task<List<Menu>> GetListMenuByTable(int id)
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.BillInfos
                    .Where(x => x.Bill.IdTable == id)
                    .Include(x => x.Food)
                    .Select(item => new Menu(
                        item.Food.Name,
                        item.Count,
                        (float)item.Food.Price,
                        (float)item.Food.Price * item.Count
                    ))
                    .ToListAsync();
            }
        }
    }
}
