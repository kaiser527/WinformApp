using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class TableFoodService
    {
        private static TableFoodService instance;

        public static TableFoodService Instance
        {
            get
            {
                if (instance == null) instance = new TableFoodService();
                return TableFoodService.instance;
            }
            private set { TableFoodService.instance = value; }
        }

        private TableFoodService() { }

        public static int TableWidth = 95;
        public static int TableHeight = 95;

        public async Task<IEnumerable<TableFood>> LoadTableList()
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.TableFoods.ToListAsync();
            }
        }

        public async Task UpdateOnChangeTableStatus(int tableId)
        {
            using (var context = new CoffeeShopContext())
            {
                TableFood table = await context.TableFoods
                    .Include(t => t.Bills)
                        .ThenInclude(t => t.BillInfos)
                    .FirstOrDefaultAsync(t => t.Id == tableId);

                if (table == null) return;

                bool hasActiveBillItems = table.Bills
                    .Any(b => b.Status == 0 && b.BillInfos.Count > 0);
                
                if (hasActiveBillItems)
                {
                    if (table.Status != "Merged") table.Status = "Reserved";
                }
                else
                {
                    table.Status = "Empty";
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<TableFood> GetTableById(int tableId)
        {
            using (var context = new CoffeeShopContext())
                return await context.TableFoods.FirstOrDefaultAsync(t => t.Id == tableId);
        }

        public async Task UpdateOnMergeTableStatus(int currentId, int targetId)
        {
            using (var context = new CoffeeShopContext())
            {
                TableFood currentTable = await context.TableFoods.FindAsync(currentId);
                TableFood targetTable = await context.TableFoods.FindAsync(targetId);

                if(currentTable == null || targetTable == null) return;

                if((currentTable.Status == "Empty" && targetTable.Status == "Reserved") || 
                    (currentTable.Status == "Reserved" && targetTable.Status == "Empty"))
                {
                    currentTable.Status = "Empty";
                    targetTable.Status = "Reserved";
                }
                else
                {
                    currentTable.Status = "Empty";
                    targetTable.Status = "Merged";
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateOnSwitchTableStatus(int currentId, int targetId)
        {
            using (var context = new CoffeeShopContext())
            {
                TableFood currentTable = await context.TableFoods.FindAsync(currentId);
                TableFood targetTable = await context.TableFoods.FindAsync(targetId);

                if (currentTable == null || targetTable == null) return;

                if (currentTable.Status == targetTable.Status) return;

                var swaps = new Dictionary<(string, string), (string, string)>
                {
                    { ("Reserved", "Merged"), ("Merged", "Reserved") },
                    { ("Merged", "Reserved"), ("Reserved", "Merged") },
                    { ("Merged", "Empty"), ("Empty", "Merged") },
                    { ("Empty", "Merged"), ("Merged", "Empty") },
                    { ("Empty", "Reserved"), ("Reserved", "Empty") },
                    { ("Reserved", "Empty"), ("Empty", "Reserved") },
                };

                if (swaps.TryGetValue((currentTable.Status, targetTable.Status), out var newStatuses))
                {
                    currentTable.Status = newStatuses.Item1;
                    targetTable.Status = newStatuses.Item2;
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
