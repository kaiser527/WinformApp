using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.DTO;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class BillService
    {
        private static BillService instance;

        public static BillService Instance
        {
            get
            {
                if (instance == null) instance = new BillService();
                return BillService.instance;
            }
            private set { BillService.instance = value; }
        }

        private BillService() { }

        public async Task<int> GetOrCreateUncheckBillIDByTableID(int tableID, string type)
        {
            using (var context = new CoffeeShopContext())
            {
                Bill bill = await context.Bills.FirstOrDefaultAsync(b => b.IdTable == tableID && b.Status == 0);

                if (bill != null)
                    return bill.Id;

                if (type == "add")
                {
                    bill = new Bill
                    {
                        IdTable = tableID,
                        DateCheckIn = DateTime.Now,
                        Status = 0
                    };

                    context.Bills.Add(bill);
                    await context.SaveChangesAsync();
                    return bill.Id;
                }

                return -1;
            }
        }

        public async Task CheckOut(int billId, float discount)
        {
            using (var context = new CoffeeShopContext())
            {
                Bill bill = await context.Bills.FindAsync(billId);

                if (bill == null || bill.Status == 1) return;

                bill.Status = 1;
                bill.Discount = discount;
                bill.DateCheckOut = DateTime.Now;

                await context.SaveChangesAsync();
            }
        }

        public async Task SwitchOrMergeTableBill(int currentTableId, int targetTableId, string action)
        {
            if (currentTableId == targetTableId) return;

            using (var context = new CoffeeShopContext())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        List<Bill> currentBills = await context.Bills
                            .Where(t => t.IdTable == currentTableId && t.Status == 0)
                            .Include(t => t.BillInfos)
                            .ToListAsync();

                        List<Bill> targetBills = await context.Bills
                            .Where(t => t.IdTable == targetTableId && t.Status == 0)
                            .Include(t => t.BillInfos)
                            .ToListAsync();

                        if (targetBills.Any(x => x.BillInfos.Count == 0) 
                            && currentBills.Any(x => x.BillInfos.Count == 0)) return;

                        if(action == "switch")
                        {
                            currentBills.ForEach(b => b.IdTable = targetTableId);
                            targetBills.ForEach(b => b.IdTable = currentTableId);
                        }
                        else if(action == "merge")
                        {
                            if (!targetBills.Any())
                            {
                                currentBills.ForEach(b => b.IdTable = targetTableId);
                            }
                            else
                            {
                                var targetBill = targetBills.First(); 

                                foreach (var currentBill in currentBills)
                                {
                                    foreach (var currentInfo in currentBill.BillInfos)
                                    {
                                        var existingInfo = targetBill.BillInfos
                                            .FirstOrDefault(bi => bi.IdFood == currentInfo.IdFood);

                                        if (existingInfo != null)
                                        {
                                            existingInfo.Count += currentInfo.Count;
                                        }
                                        else
                                        {
                                            targetBill.BillInfos.Add(new BillInfo
                                            {
                                                IdFood = currentInfo.IdFood,
                                                Count = currentInfo.Count,
                                                IdBill = targetBill.Id
                                            });
                                        }
                                    }
                                    context.Bills.Remove(currentBill);
                                }
                            }
                        }
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<BillDTO>> GetCheckedBillByDate(DateTime dateCheckIn, DateTime dateCheckOut)
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.Bills
                    .Where(b => b.DateCheckIn >= dateCheckIn && b.DateCheckOut <= dateCheckOut && b.Status == 1)
                    .Select(b => new BillDTO(
                        b.Table.Name,
                        b.Discount,
                        (float)(b.BillInfos.Sum(x => x.Count * x.Food.Price)) * (1 - b.Discount),
                        b.DateCheckIn,
                        b.DateCheckOut ?? DateTime.Now
                    ))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Bill>> GetListCheckBill()
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.Bills
                    .Where(b => b.Status == 1)
                    .ToListAsync();
            }
        }
    }
}
