using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormApp.DTO;
using WinFormApp.Jobs;
using WinFormApp.Models;
using WinFormApp.Services;

namespace WinFormApp
{
    public partial class TableManager : Form
    {
        public TableManager()
        {
            InitializeComponent();
            Load += load_Data;
            FormClosing += TableManager_FormClosing;
        }

        #region Method
        public async Task LoadCategory()
        {
            IEnumerable<FoodCategory> foodCategories = await CategoryService.Instance.GetListCategory();
            cbCategory.DataSource = foodCategories;
            cbCategory.DisplayMember = "Name";
        }
        private async Task LoadFoodListByCategoryID(int id)
        {
            IEnumerable<Food> foods = await FoodService.Instance.GetListFoodByCategoryID(id);
            cbFood.DataSource = foods;
            cbFood.DisplayMember = "Name";
        }
        public async Task LoadTableFood()
        {
            flpTable.Controls.Clear();

            var role = AccountService.Instance.User.Role;

            if (role == null || !role.IsActive) return;

            var permissionNames = role.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToList();

            IEnumerable<TableFood> tables = permissionNames.Contains("View Table") ? 
                await TableFoodService.Instance.LoadTableList() : null;

            if(tables == null) return;

            foreach (TableFood table in tables)
            {
                Button btn = new Button()
                {
                    Width = TableFoodService.TableWidth,
                    Height = TableFoodService.TableHeight,
                    Text = table.Name + Environment.NewLine + table.Status,
                };

                btn.Click += btn_Click;
                btn.Tag = table;
                        
                switch (table.Status)
                {
                    case "Empty":
                        btn.BackColor = Color.LightGreen;
                        break;
                    case "Reserved":
                        btn.BackColor = Color.Goldenrod;
                        break;
                    case "Merged":
                        btn.BackColor = Color.IndianRed;
                        break;
                    default:
                        btn.BackColor = Color.LightGray;
                        break;
                }

                flpTable.Controls.Add(btn);
            }

            cbSwitch.DataSource = tables;
            cbSwitch.DisplayMember = "Name";
        }
        private async Task ShowBill(int tableId)
        {
            lsvBill.Items.Clear();

            IEnumerable<MenuDTO> billInfos = await BillInfoService.Instance.GetListMenuByTable(tableId);

            float totalPrice = 0;

            CultureInfo culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;

            foreach (MenuDTO billInfo in billInfos)
            {
                ListViewItem listViewItem = new ListViewItem(billInfo.FoodName);
                listViewItem.SubItems.Add(billInfo.Count.ToString());
                listViewItem.SubItems.Add(billInfo.Price.ToString("c", culture));
                listViewItem.SubItems.Add(billInfo.TotalPrice.ToString("c", culture));

                totalPrice += billInfo.TotalPrice;

                lsvBill.Items.Add(listViewItem);
            }

            txbTotalPrice.Tag = Convert.ToDecimal(totalPrice);
            txbTotalPrice.Text = totalPrice.ToString("c", culture);

            ApplyDiscount();
        }
        public void UpdateTableButtonColor(TableFood table)
        {
            Button btn = flpTable.Controls
                .OfType<Button>()
                .FirstOrDefault(b => (b.Tag as TableFood).Id == table.Id);

            if (btn == null) return;

            btn.Text = table.Name + Environment.NewLine + table.Status;

            switch (table.Status)
            {
                case "Empty":
                    btn.BackColor = Color.LightGreen;
                    break;
                case "Reserved":
                    btn.BackColor = Color.Goldenrod;
                    break;
                case "Merged":
                    btn.BackColor = Color.IndianRed;
                    break;
                default:
                    btn.BackColor = Color.LightGray;
                    break;
            }
        }

        private void ApplyDiscount()
        {
            if (txbTotalPrice.Tag == null) return;

            CultureInfo culture = new CultureInfo("en-US");

            decimal originalTotal = (decimal)txbTotalPrice.Tag;  
            decimal discountPercent = nmDiscount.Value;

            decimal finalTotal = originalTotal * (1 - discountPercent / 100m);

            txbTotalPrice.Text = finalTotal.ToString("c", culture);
        }
        private async Task HandleTableAction(string action)
        {
            TableFood currentTable = lsvBill.Tag as TableFood;
            TableFood targetTable = cbSwitch.SelectedItem as TableFood;

            if (currentTable == null || targetTable == null) return;

            string message = action == "switch"
                ? $"Are you sure you want to switch all bills between {currentTable.Name} and {targetTable.Name}?"
                : $"Are you sure you want to merge all bills from {currentTable.Name} into {targetTable.Name}?";

            DialogResult result = MessageBox.Show(
                message,
                $"Confirm {action}",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.OK) return;

            await BillService.Instance.SwitchOrMergeTableBill(currentTable.Id, targetTable.Id, action);

            if(action == "switch")
            {
                await TableFoodService.Instance.UpdateOnSwitchTableStatus(currentTable.Id, targetTable.Id);
            }
            else
            {
                await TableFoodService.Instance.UpdateOnMergeTableStatus(currentTable.Id, targetTable.Id);
            }

            currentTable = await TableFoodService.Instance.GetTableById(currentTable.Id);
            targetTable = await TableFoodService.Instance.GetTableById(targetTable.Id);

            UpdateTableButtonColor(currentTable);
            UpdateTableButtonColor(targetTable);

            await ShowBill(currentTable.Id);
        }

        private async Task StartImageCleanupScheduler()
        {
            try
            {
                // Create a Quartz scheduler
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                // Define the job
                IJobDetail job = JobBuilder.Create<ImageCleanupJob>()
                    .WithIdentity("ImageCleanupJob", "Maintenance")
                    .Build();

                // Define the trigger (every 5 minutes)
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("ImageCleanupTrigger", "Maintenance")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(5)
                        .RepeatForever())
                    .Build();

                // Schedule it
                await scheduler.ScheduleJob(job, trigger);

                Console.WriteLine("[Quartz] Image cleanup job scheduled every 5 minutes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Failed to start scheduler: {ex.Message}");
            }
        }
        #endregion

        #region Event
        private async void TableManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            if (scheduler.IsStarted) await scheduler.Shutdown();
        }
        private async void load_Data(object sender, EventArgs e)
        {
            var tableTask = LoadTableFood();
            var categoryTask = LoadCategory();

            Account user = AccountService.Instance.User;

            if (user == null)
            {
                MessageBox.Show("User is null!");
                return;
            }

            adminLabel.Visible = user.Role.Name == "Admin" || user.Role.Name == "Tester";

            accountToolStripDropdown.Text += $" ({user.DisplayName})";

            Image img = AccountService.Instance.LoadAccountImage(user, 38);
            if (img == null)
            {
                MessageBox.Show("Image not found or failed to load!");
            }
            else
            {
                AccountImage.Image = img;
                AccountImage.SizeMode = PictureBoxSizeMode.Zoom;
            }

            await Task.WhenAll(tableTask, categoryTask);

            await StartImageCleanupScheduler();
        }
        private async void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as TableFood).Id;
            lsvBill.Tag = (sender as Button).Tag;
            await ShowBill(tableID);
        }
        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void privateInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccoutProfile accoutProfile = new AccoutProfile(this);
            accoutProfile.ShowDialog();
        }
        private void adminLabel_Click(object sender, EventArgs e)
        {
            Admin admin = new Admin(this);
            admin.ShowDialog();
        }
        private async void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null) return;

            FoodCategory selected = cb.SelectedItem as FoodCategory;

            await LoadFoodListByCategoryID(selected.Id);
        }
        private async void btnAddFood_Click(object sender, EventArgs e)
        {
            TableFood table = lsvBill.Tag as TableFood;

            if (table == null) return;

            int idBill = await BillService.Instance.GetOrCreateUncheckBillIDByTableID(table.Id, "add");
            int idFood = (cbFood.SelectedItem as Food).Id;
            int count = (int)nmAddFood.Value;

            await BillInfoService.Instance.InsertBillInfo(idBill, idFood, count);

            await TableFoodService.Instance.UpdateOnChangeTableStatus(table.Id);

            table = await TableFoodService.Instance.GetTableById(table.Id);

            UpdateTableButtonColor(table);

            await ShowBill(table.Id);
        }
        private async void btnCheckout_Click(object sender, EventArgs e)
        {
            TableFood table = lsvBill.Tag as TableFood;

            if (table == null) return;

            int discount = (int)nmDiscount.Value;
            int idBill = await BillService.Instance.GetOrCreateUncheckBillIDByTableID(table.Id, "checkout");
            decimal finalTotal = (decimal)txbTotalPrice.Tag * (1 - nmDiscount.Value / 100m);
            string resultTotal = discount == 0 ?
                $"total price: {txbTotalPrice.Tag}$" :
                $"discount:\n{txbTotalPrice.Tag}$ - {txbTotalPrice.Tag}$ * {discount}% = {finalTotal}$";

            if (idBill != -1)
            {
                string msg = $"Are you sure want to check out for {table.Name} with {resultTotal}";

                if (MessageBox.Show(msg, "Alert", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    await BillService.Instance.CheckOut(idBill, (float)(discount * 0.01));

                    await TableFoodService.Instance.UpdateOnChangeTableStatus(table.Id);

                    table = await TableFoodService.Instance.GetTableById(table.Id);

                    UpdateTableButtonColor(table);

                    await ShowBill(table.Id);
                }
            }
        }
        private void nmDiscount_ValueChanged(object sender, EventArgs e)
        {
            ApplyDiscount();
        }
        private async void btnSwitchTable_Click(object sender, EventArgs e)
        {
            await HandleTableAction("switch");
        }
        private async void btnMergeTable_Click(object sender, EventArgs e)
        {
            await HandleTableAction("merge");
        }
        #endregion
    }
}
