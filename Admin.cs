using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormApp.DTO;
using WinFormApp.Models;
using WinFormApp.Services;

namespace WinFormApp
{
    public partial class Admin : Form
    {
        private readonly TableManager _tableManager;

        private BindingSource foodlist = new BindingSource();

        private BindingSource rolelist = new BindingSource();

        private List<Role> _roles;

        private Role _selectedRole;

        public Admin(TableManager tableManager)
        {
            InitializeComponent();
            ApplyPermissions();
            Load += LoadData;
            _tableManager = tableManager;
        }

        #region methods
        private async Task LoadBillList(DateTime checkIn, DateTime checkOut)
        {
            IEnumerable<BillDTO> bills = await BillService.Instance.GetCheckedBillByDate(checkIn, checkOut);
            dtgvBill.DataSource = bills;
        }

        private async Task LoadCategoryList()
        {
            cbFoodCategory.DataSource = await CategoryService.Instance.GetListCategory();
            cbFoodCategory.DisplayMember = "Name";
        }

        private void ApplyPermissions()
        {
            var permissionNames = AccountService.Instance.User.Role.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToList();

            var permissionActions = new Dictionary<string, Action>
            {
                // Bill
                { "View Bill", () => tcAdmin.TabPages.Remove(tpBill) },

                // Food
                { "View Food", () => tcAdmin.TabPages.Remove(tpFood) },
                { "Create Food", () => btnAddFood.Enabled = false },
                { "Delete Food", () => btnDeleteFood.Enabled = false },
                { "Update Food", () => btnUpdateFood.Enabled = false },

                // Category
                { "View Category", () => tcAdmin.TabPages.Remove(tpCategory) },
                { "Create Category", () => btnAddCategory.Enabled = false },
                { "Delete Category", () => btnDeleteCategory.Enabled = false },
                { "Update Category", () => btnUpdateCategory.Enabled = false },

                // Table
                { "View Table", () => tcAdmin.TabPages.Remove(tpTable) },
                { "Create Table", () => btnAddTable.Enabled = false },
                { "Delete Table", () => btnDeleteTable.Enabled = false },
                { "Update Table", () => btnUpdateTable.Enabled = false },

                // Account
                { "View Account", () => tcAdmin.TabPages.Remove(tpAccount) },
                { "Create Account", () => btnAddAccount.Enabled = false },
                { "Delete Account", () => btnDeleteAccount.Enabled = false },
                { "Update Account", () => btnUpdateAccount.Enabled = false },

                //Role
                { "View Role", () => tcAdmin.TabPages.Remove(tpRole) },
                { "Create Role", () => btnAddRole.Enabled = false },
                { "Delete Role", () => btnDeleteRole.Enabled = false },
                { "Update Role", () => btnUpdateRole.Enabled = false },

                //Permission
                { "View Permission", () => tcAdmin.TabPages.Remove(tpPermission) },
                //{ "Create Permission", () => btnAddPermission.Enabled = false },
                //{ "Delete Permission", () => btnDeletePermissione.Enabled = false },
                //{ "Update Permission", () => btnUpdatePermission.Enabled = false }
            };

            foreach (var kvp in permissionActions)
            {
                if (!permissionNames.Contains(kvp.Key))
                {
                    kvp.Value.Invoke();
                }
            }
        }

        private async Task LoadBillListDefault()
        {
            IEnumerable<Bill> bills = await BillService.Instance.GetListCheckBill();

            var minDateCheckIn = bills.Min(x => x.DateCheckIn);
            var maxDateCheckOut = bills.Max(x => x.DateCheckOut);

            dtpkFromDate.Value = minDateCheckIn;
            dtpkToDate.Value = maxDateCheckOut ?? System.DateTime.Now;

            var billTask = LoadBillList(dtpkFromDate.Value, dtpkToDate.Value);

            var categoryTask = LoadCategoryList();

            await Task.WhenAll(billTask, categoryTask);
        }

        public async Task LoadGroupPermission()
        {
            var permissions = await PermissionService.Instance.GetListPermission();

            var groupedPermissions = permissions
                .GroupBy(p => p.Module)
                .Select(g => new
                {
                    Module = g.Key,
                    Permissions = g.ToList()
                })
                .ToList();

            flpRole.Controls.Clear();
            flpRole.FlowDirection = FlowDirection.LeftToRight;
            flpRole.WrapContents = true;
            flpRole.AutoScroll = true;

            int moduleWidth = (flpRole.Width - flpRole.Padding.Horizontal - 20) / 3;

            foreach (var group in groupedPermissions)
            {
                var modulePanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,
                    Width = moduleWidth,
                    AutoSize = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(5),
                    Margin = new Padding(5)
                };

                var headerPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true
                };

                var btnToggle = new Button
                {
                    Text = "▶",
                    Width = 30,
                    Height = 25
                };

                var moduleCheckBox = new CheckBox
                {
                    Text = group.Module,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                headerPanel.Controls.Add(btnToggle);
                headerPanel.Controls.Add(moduleCheckBox);

                modulePanel.Controls.Add(headerPanel);

                var permissionPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,
                    AutoSize = true,
                    Margin = new Padding(20, 0, 0, 0),
                    Visible = false
                };

                List<EventHandler> permHandlers = new List<EventHandler>();

                foreach (var perm in group.Permissions)
                {
                    var permCheckBox = new CheckBox
                    {
                        Text = perm.Name,
                        AutoSize = true,
                        Tag = perm
                    };

                    EventHandler handler = (s, ev) =>
                    {
                        bool allChecked = permissionPanel.Controls.OfType<CheckBox>().All(c => c.Checked);

                        moduleCheckBox.CheckedChanged -= ModuleCheckBoxHandler;
                        moduleCheckBox.Checked = allChecked;
                        moduleCheckBox.CheckedChanged += ModuleCheckBoxHandler;
                    };

                    permCheckBox.CheckedChanged += handler;
                    permHandlers.Add(handler);

                    permissionPanel.Controls.Add(permCheckBox);
                }

                void ModuleCheckBoxHandler(object sender, EventArgs e)
                {
                    for (int i = 0; i < permissionPanel.Controls.Count; i++)
                    {
                        var c = (CheckBox)permissionPanel.Controls[i];
                        c.CheckedChanged -= permHandlers[i]; // detach temporarily
                        c.Checked = moduleCheckBox.Checked;
                        c.CheckedChanged += permHandlers[i]; // reattach
                    }
                }

                moduleCheckBox.CheckedChanged += ModuleCheckBoxHandler;

                btnToggle.Click += (s, ev) =>
                {
                    permissionPanel.Visible = !permissionPanel.Visible;
                    btnToggle.Text = permissionPanel.Visible ? "▼" : "▶";
                };

                modulePanel.Controls.Add(permissionPanel);

                flpRole.Controls.Add(modulePanel);
            }
        }

        private void ApplyRolePermissionsToUI()
        {
            if (_selectedRole == null) return;

            var rolePermissionNames = _selectedRole.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToHashSet();

            foreach (var modulePanel in flpRole.Controls.OfType<FlowLayoutPanel>())
            {
                foreach (var permPanel in modulePanel.Controls.OfType<FlowLayoutPanel>())
                {
                    foreach (var cb in permPanel.Controls.OfType<CheckBox>())
                    {
                        if (cb.Tag is Permission perm)
                        {
                            cb.Checked = rolePermissionNames.Contains(perm.Name);
                        }
                    }
                }
            }
        }

        public async Task InsertOrUpdateRole(Func<Role, Task> action)
        {
            var selectedPermissions = new List<Permission>();

            foreach (var modulePanel in flpRole.Controls.OfType<FlowLayoutPanel>())
            {
                foreach (var permissionPanel in modulePanel.Controls.OfType<FlowLayoutPanel>())
                {
                    foreach (var cb in permissionPanel.Controls.OfType<CheckBox>())
                    {
                        if (cb.Checked && cb.Tag is Permission perm)
                        {
                            selectedPermissions.Add(perm);
                        }
                    }
                }
            }

            var newRole = new Role
            {
                Id = string.IsNullOrWhiteSpace(txbRoleId.Text) ? 0 : int.Parse(txbRoleId.Text),
                Name = txbRoleName.Text,
                IsActive = cbIsActiveRole.SelectedItem?.ToString() == "Active",
                RolePermissions = selectedPermissions
                    .Select(p => new RolePermission { PermissionId = p.Id })
                    .ToList()
            };

            await action(newRole);  

            _roles = (await RoleService.Instance.GetListRole(txbSearchRole.Text)).ToList();

            var rolesWithString = _roles.Select(r => new
            {
                r.Id,
                r.Name,
                IsActive = r.IsActive ? "Active" : "Inactive",
                Permissions = string.Join(", ", r.RolePermissions.Select(rp => rp.Permission.Name))
            }).ToList();

            rolelist.DataSource = rolesWithString;
        }
        #endregion

        #region events
        public async void LoadData(object sender, EventArgs e)
        {
            var billTask = LoadBillListDefault();
            var permissionTask = LoadGroupPermission();

            await Task.WhenAll(billTask, permissionTask);

            cbIsActiveRole.DataSource = new List<string> { "Active", "Inactive" };
        }

        private async void btnViewBill_Click(object sender, EventArgs e)
        {
            await LoadBillList(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private async void btnViewFood_Click(object sender, EventArgs e)
        {
            foodlist.DataSource = await FoodService.Instance.GetListFood();

            dtgvFood.DataSource = foodlist;

            txbFoodName.DataBindings.Clear();
            txbFoodId.DataBindings.Clear();
            cbFoodCategory.DataBindings.Clear();
            nmFoodPrice.DataBindings.Clear();

            txbFoodName.DataBindings.Add("Text", foodlist, "Name", true, DataSourceUpdateMode.Never);
            txbFoodId.DataBindings.Add("Text", foodlist, "Id", true, DataSourceUpdateMode.Never);
            cbFoodCategory.DataBindings.Add("Text", foodlist, "Category", true, DataSourceUpdateMode.Never);
            nmFoodPrice.DataBindings.Add("Value", foodlist, "Price", true, DataSourceUpdateMode.Never);
        }

        private async void btnAddFood_Click(object sender, EventArgs e)
        {
            FoodDTO food = new FoodDTO(0, txbFoodName.Text, cbFoodCategory.Text, (float)nmFoodPrice.Value);

            await FoodService.Instance.InsertFood(food);

            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);

            await _tableManager.LoadCategory();
        }

        private async void btnUpdateFood_Click(object sender, EventArgs e)
        {
            FoodDTO food = new FoodDTO(int.Parse(txbFoodId.Text), txbFoodName.Text, cbFoodCategory.Text, (float)nmFoodPrice.Value);

            await FoodService.Instance.UpdateFood(food);

            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);

            await _tableManager.LoadCategory();
        }

        private async void btnDeleteFood_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this food?",
               "Delete Confirmation",
               MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            List<TableFood> tables = await FoodService.Instance.DeleteFood(int.Parse(txbFoodId.Text));

            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);

            await _tableManager.LoadCategory();  
            
            foreach (TableFood tableFood in tables)
            {
                _tableManager.UpdateTableButtonColor(tableFood);
            }
        }

        private async void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);
        }

        private async void btnViewRole_Click(object sender, EventArgs e)
        {
            _roles = (await RoleService.Instance.GetListRole(txbSearchRole.Text)).ToList();

            var rolesWithString = _roles.Select(r => new
            {
                r.Id,
                r.Name,
                IsActive = r.IsActive ? "Active" : "Inactive",
                Permissions = string.Join(", ", r.RolePermissions.Select(rp => rp.Permission.Name))
            }).ToList();

            rolelist.DataSource = rolesWithString;
            dtgvRole.DataSource = rolelist;
            dtgvRole.Columns["Permissions"].Visible = false;

            txbRoleId.DataBindings.Clear();
            txbRoleName.DataBindings.Clear();
            cbIsActiveRole.DataBindings.Clear();

            txbRoleId.DataBindings.Add("Text", rolelist, "Id", true, DataSourceUpdateMode.Never);
            txbRoleName.DataBindings.Add("Text", rolelist, "Name", true, DataSourceUpdateMode.Never);
            cbIsActiveRole.DataBindings.Add("SelectedItem", rolelist, "IsActive", true, DataSourceUpdateMode.Never);
        }

        private void dtgvRole_SelectionChanged(object sender, EventArgs e)
        {
            if (dtgvRole.CurrentRow != null)
            {
                int roleId = (int)dtgvRole.CurrentRow.Cells["Id"].Value;
                _selectedRole = _roles.FirstOrDefault(r => r.Id == roleId);
                ApplyRolePermissionsToUI();
            }
        }

        private async void btnAddRole_Click(object sender, EventArgs e)
        {
            await InsertOrUpdateRole(RoleService.Instance.InsertRole);
        }

        private async void btnUpdateRole_Click(object sender, EventArgs e)
        {
            await InsertOrUpdateRole(RoleService.Instance.UpdateRole);
        }

        private async void btnDeleteRole_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this role?",
               "Delete Confirmation",
               MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            await RoleService.Instance.DeleteRole(int.Parse(txbRoleId.Text));

            _roles = (await RoleService.Instance.GetListRole(txbSearchRole.Text)).ToList();

            var rolesWithString = _roles.Select(r => new
            {
                r.Id,
                r.Name,
                IsActive = r.IsActive ? "Active" : "Inactive",
                Permissions = string.Join(", ", r.RolePermissions.Select(rp => rp.Permission.Name))
            }).ToList();

            rolelist.DataSource = rolesWithString;
        }
        #endregion
    }
}
