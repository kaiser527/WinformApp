using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public static int TableWidth = 106;
        public static int TableHeight = 106;

        public async Task<IEnumerable<TableFood>> LoadTableList(string name = null)
        {
            using (var context = new CoffeeShopContext())
            {
                var query = context.TableFoods.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(f => f.Name.ToLower().Contains(name.ToLower()));
                }

                return await query.ToListAsync();
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

                if (currentTable.Status == "Empty" && currentTable.Status == "Empty") return;

                if ((currentTable.Status == "Empty" && targetTable.Status == "Reserved") || 
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

        public async Task InsertTable(TableFood tableFood)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isValid = Regex.IsMatch(tableFood.Name, @"^Table\s\d+$", RegexOptions.IgnoreCase);

                if (!isValid)
                {
                    MessageBox.Show("Table name format is incorrect", "Insert failed");
                    return;
                }

                var parts = tableFood.Name.Split(' ');

                bool isExist = await context.TableFoods.AnyAsync(t => t.Name.EndsWith(" " + parts[1]));

                if (isExist)
                {
                    MessageBox.Show("Table is already exist", "Insert failed");
                    return;
                }

                context.TableFoods.Add(tableFood);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateTable(TableFood tableFood)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isValid = Regex.IsMatch(tableFood.Name, @"^Table\s\d+$", RegexOptions.IgnoreCase);

                if (!isValid)
                {
                    MessageBox.Show("Table name format is incorrect", "Update failed");
                    return;
                }

                TableFood updateTable = await context.TableFoods.FindAsync(tableFood.Id);

                if (updateTable == null)
                {
                    MessageBox.Show("Table is not exist", "Update failed");
                    return;
                }

                var parts = tableFood.Name.Split(' ');

                bool isExist = await context.TableFoods
                    .AnyAsync(t => t.Name.EndsWith(" " + parts[1]) && t.Id != tableFood.Id);

                if (isExist)
                {
                    MessageBox.Show("Table is already exist", "Update failed");
                    return;
                }

                updateTable.Name = tableFood.Name;
                updateTable.Status = tableFood.Status;
                
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteTable(int tableId)
        {
            using (var context = new CoffeeShopContext())
            {
                TableFood table = await context.TableFoods.FindAsync(tableId);

                if (table == null)
                {
                    MessageBox.Show("Table is not exist", "Delete failed");
                    return;
                }

                context.TableFoods.Remove(table);

                await context.SaveChangesAsync();   
            }
        }
    }
}
