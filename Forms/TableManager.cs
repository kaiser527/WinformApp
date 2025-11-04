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
using WinFormApp.Utils;
using WinFormApp.Models;
using WinFormApp.Services;
using WinFormApp.Forms;

namespace WinFormApp
{
    public partial class TableManager : Form
    {
        private static IScheduler _scheduler;

        private int _currentPage = 1;

        private readonly int _pageSize = 20;

        private int _totalPages = 1;

        private readonly bool _isPlaceholderApplied = false;

        public TableManager()
        {
            InitializeComponent();
            StylePanels();
            StyleButtons();
            StyleListView();
            UIStyles.ApplyPlaceholder(txbSearchTable, "Search table...", ref _isPlaceholderApplied);
            Load += load_Data;
            FormClosing += TableManager_FormClosing;
        }

        #region Methods
        private void ApplyButtonColor(string status, Button btn)
        {
            switch (status)
            {
                case "Empty": btn.BackColor = Color.LightGreen; break;
                case "Reserved": btn.BackColor = Color.Goldenrod; break;
                case "Merged": btn.BackColor = Color.IndianRed; break;
                default: btn.BackColor = Color.LightGray; break;
            }
        }
        private void StyleListView()
        {
            AutoResizeBillColumns();
            lsvBill.SizeChanged += (s, e) => AutoResizeBillColumns();
            UIStyles.RoundListView(lsvBill, 15);
        }
        public async Task LoadCategory()
        {
            var result = await CategoryService.Instance.GetListCategory();
            cbCategory.DataSource = result.Items;
            cbCategory.DisplayMember = "Name";
        }
        private async Task LoadFoodListByCategoryID(int id)
        {
            IEnumerable<Food> foods = await FoodService.Instance.GetListFoodByCategoryID(id);
            cbFood.DataSource = foods;
            cbFood.DisplayMember = "Name";
        }
        private void StylePanels()
        {
            int radius = 15;

            foreach (Panel pnl in new[] { flpTable, panel5, paginatePanel, panel3 })
            {
                UIStyles.RoundPanel(pnl, radius);
            }
        }
        private void AutoResizeBillColumns()
        {
            if (lsvBill.Columns.Count == 0) return;

            int columnWidth = lsvBill.ClientSize.Width / lsvBill.Columns.Count;

            foreach (ColumnHeader col in lsvBill.Columns) col.Width = columnWidth;
        }
        private void StyleButtons()
        {
            UIStyles.ModernUIButton(btnAddFood, Color.FromArgb(46, 204, 113), Color.FromArgb(22, 160, 133));
            UIStyles.ModernUIButton(btnSwitchTable, Color.FromArgb(243, 156, 18), Color.FromArgb(211, 84, 0));
            UIStyles.ModernUIButton(btnMergeTable, Color.FromArgb(155, 89, 182), Color.FromArgb(125, 60, 152));
            UIStyles.ModernUIButton(btnCheckout, Color.FromArgb(231, 76, 60), Color.FromArgb(176, 52, 40));
        }
        public async Task LoadTableFood()
        {
            flpTable.Controls.Clear();

            var role = AccountService.Instance.User.Role;

            if (role == null || !role.IsActive) return;

            var permissionNames = role.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToList();

            string queryName = txbSearchTable.Text == "Search table..." ?
                null : txbSearchTable.Text;

            var result = permissionNames.Contains("View Table") ?
                await TableFoodService.Instance.LoadTableList(_pageSize, _currentPage, queryName) : null;

            if (result == null || !result.Items.Any()) return;

            foreach (TableFood table in result.Items)
            {
                Button btn = new Button()
                {
                    Width = TableFoodService.TableWidth,
                    Height = TableFoodService.TableHeight,
                    Text = table.Name + Environment.NewLine + table.Status,
                };

                btn.Click += btn_Click;
                btn.Tag = table;

                ApplyButtonColor(table.Status, btn);    

                flpTable.Controls.Add(btn);

                flpTable.Padding = new Padding(10, 1, 0, 0);
            }

            cbSwitch.DataSource = permissionNames.Contains("View Table") ?
                (await TableFoodService.Instance.LoadTableList(100, 1, queryName)).Items : null;

            cbSwitch.DisplayMember = "Name";

            _totalPages = result.TotalPages;

            LayoutForm.RenderPagination(
                paginatePanel,
                _currentPage, 
                _totalPages,
                async (newPage) =>
                {
                    _currentPage = newPage;
                    await LoadTableFood();   
                }
            );
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

            ApplyButtonColor(table.Status, btn);
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

            bool confirm = Confirmation.ShowConfirm($"Confirm {action}", message);

            if (!confirm) return;

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
                if (_scheduler != null && _scheduler.IsStarted) return;

                _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await _scheduler.Start();

                IJobDetail job = JobBuilder.Create<ImageCleanupJob>()
                    .WithIdentity("ImageCleanupJob", "Maintenance")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("ImageCleanupTrigger", "Maintenance")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);

                Console.WriteLine("[Quartz] Image cleanup scheduled every 5 minutes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Quartz] Failed: {ex.Message}");
            }
        }
        #endregion

        #region Events
        private async void TableManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_scheduler != null && _scheduler.IsStarted)
                await _scheduler.Shutdown();
        }
        private async void load_Data(object sender, EventArgs e)
        {
            var tableTask = LoadTableFood();
            var categoryTask = LoadCategory();

            Account user = AccountService.Instance.User;

            if (user == null)
            {
                Alert.ShowAlert("Account is not exist", Alert.AlertType.Error);
                return;
            }

            adminLabel.Visible = user.Role.Name == "Admin" || user.Role.Name == "Tester";

            accountToolStripDropdown.Text += $" ({user.DisplayName})";

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
                $"Total price: {txbTotalPrice.Tag}$" :
                $"Discount:\n{txbTotalPrice.Tag}$ - {txbTotalPrice.Tag}$ * {discount}% = {finalTotal}$";

            if (idBill != -1)
            {
                string msg = $"Are you sure you want to check out table {table.Name}?\n\n{resultTotal}";

                // ✅ show custom confirmation dialog
                bool confirm = Confirmation.ShowConfirm("Checkout", msg);

                if (!confirm) return;

                // ✅ continue checkout if confirmed
                await BillService.Instance.CheckOut(idBill, (float)(discount * 0.01));

                await TableFoodService.Instance.UpdateOnChangeTableStatus(table.Id);

                table = await TableFoodService.Instance.GetTableById(table.Id);

                UpdateTableButtonColor(table);

                await ShowBill(table.Id);
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
        private async void txbSearchTable_TextChanged(object sender, EventArgs e)
        {
            if (_isPlaceholderApplied) return;

            _currentPage = 1;

            await LoadTableFood();
        }
        #endregion
    }
}
