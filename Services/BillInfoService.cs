using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.Models;
using WinFormApp.DTO;

namespace WinFormApp.Services
{
    internal class BillInfoService
    {
        private static BillInfoService instance;

        public static BillInfoService Instance
        {
            get
            {
                if (instance == null) instance = new BillInfoService();
                return BillInfoService.instance;
            }
            private set { BillInfoService.instance = value; }
        }

        public async Task<IEnumerable<MenuDTO>> GetListMenuByTable(int id)
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.BillInfos
                    .Where(x => x.Bill.IdTable == id && x.Bill.Status == 0)
                    .Select(item => new MenuDTO(
                        item.Food.Name,
                        item.Count,
                        (float)item.Food.Price,
                        (float)item.Food.Price * item.Count
                    ))
                    .ToListAsync();
            }
        }

        public async Task InsertBillInfo(int billId, int foodId, int count)
        {
            using (var context = new CoffeeShopContext())
            {
                BillInfo currentBillInfo = await context.BillInfos.FirstOrDefaultAsync(x => x.IdBill == billId && x.IdFood == foodId);

                if (currentBillInfo == null)
                {
                    if (count > 0)
                    {
                        var billInfo = new BillInfo
                        {
                            IdBill = billId,
                            IdFood = foodId,
                            Count = count
                        };
                        context.BillInfos.Add(billInfo);
                    }
                }
                else
                {
                    currentBillInfo.Count += count;

                    if (currentBillInfo.Count <= 0)
                    {
                        context.BillInfos.Remove(currentBillInfo);
                    }
                }

                await context.SaveChangesAsync();

                Bill bill = await context.Bills
                       .Include(b => b.BillInfos)
                       .FirstOrDefaultAsync(b => b.Id == billId);

                if (bill != null && !bill.BillInfos.Any())
                {
                    context.Bills.Remove(bill);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
