using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormApp.DTO;
using WinFormApp.Forms;
using WinFormApp.Models;
using WinFormApp.Services;
using WinFormApp.Utils;

namespace WinFormApp
{
    public partial class Admin : Form
    {
        //Dependency injection
        private readonly TableManager _tableManager;

        //Data binding
        private BindingSource foodlist = new BindingSource();
        private BindingSource rolelist = new BindingSource();
        private BindingSource permissionlist = new BindingSource();
        private BindingSource accountlist = new BindingSource();
        private BindingSource tablelist = new BindingSource();
        private BindingSource categorylist = new BindingSource();

        //Role
        private List<Role> _roles;
        private Role _selectedRole;

        //File
        private string _filename;

        //Paginate
        private int _pageSize = 15;
        //Food
        private int _currentPageFood = 1;
        private int _totalPagesFood = 1;
        //Category
        private int _currentPageCategory = 1;
        private int _totalPagesCategory = 1;
        //Table
        private int _currentPageTable = 1;
        private int _totalPagesTable = 1;
        //Account
        private int _currentPageAccount = 1;
        private int _totalPagesAccount = 1;
        //Role
        private int _currentPageRole = 1;
        private int _totalPagesRole = 1;
        //Permission
        private int _currentPagePermission = 1;
        private int _totalPagesPermission = 1;

        public Admin(TableManager tableManager)
        {
            InitializeComponent();
            ApplyPermissions();
            StylePanels();
            Load += LoadData;
            _tableManager = tableManager;
        }

        #region Methods
        private void StylePanels()
        {
            foreach (var panel in (new Panel[]
            { 
                //food
                paginatePanelFood,
                panel3, panel4, panel5, panel6,
                panel7, panel8, panel9, panel10,
                //category
                paginatePanelCategory,
                panel29, panel11, panel13, panel30,
                panel28, panel27,
                //Table
                paginatePanelTable,
                panel14, panel15, panel19, panel12,
                panelTableName, panel17, panel22,
                //Account
                paginatePanelAccount,
                panel16, panel18, panel20, panel21,
                panel24, panel25, panel26,
                //Role
                paginatePanelRole,
                panel23, panel32, panel33, panel31,
                flpRole, panel37, panel36, panel34,
                //Permission
                paginatePanelPermission,
                panel39, panel35, panel40, panel41,
                panel42, panel43, panel44,
            }).Cast<Panel>())
            {
                UIStyles.RoundPanel(panel, 15);
            }
        }
        private async Task LoadBillList(DateTime checkIn, DateTime checkOut)
        {
            IEnumerable<BillDTO> bills = await BillService.Instance.GetCheckedBillByDate(checkIn, checkOut);
            dtgvBill.DataSource = bills;
        }

        private async Task LoadCategoryList()
        {
            cbFoodCategory.DataSource = (await CategoryService.Instance.GetListCategory()).Items;
            cbFoodCategory.DisplayMember = "Name";
        }

        private void ApplyPermissions()
        {
            var role = AccountService.Instance.User.Role;

            if (role == null || !role.IsActive)
            {
                Alert.ShowAlert(
                    "Your role is inactive. You do not have permission to access this area.", 
                    Alert.AlertType.Error
                );

                tcAdmin.TabPages.Clear();

                return;
            }

            var permissionNames = role.RolePermissions
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
                { "Create Permission", () => btnAddPermission.Enabled = false },
                { "Delete Permission", () => btnDeletePermission.Enabled = false },
                { "Update Permission", () => btnUpdatePermission.Enabled = false }
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
            dtpkToDate.Value = maxDateCheckOut ?? DateTime.Now;

            var billTask = LoadBillList(dtpkFromDate.Value, dtpkToDate.Value);

            var categoryTask = LoadCategoryList();

            await Task.WhenAll(billTask, categoryTask);
        }

        public async Task LoadGroupPermission()
        {
            var result = await PermissionService.Instance.GetListPermission();

            var groupedPermissions = result.Items
                .GroupBy(p => p.Module)
                .Select(g => new
                {
                    Module = g.Key,
                    Permissions = g.ToList()
                })
                .ToList();

            flpRole.Controls.Clear();
            flpRole.AutoScroll = true;

            // ✅ Add padding around modules
            flpRole.Padding = new Padding(13);

            // ✅ 2 columns compact layout
            int columns = 2;
            int spacing = 8;

            // ✅ Width after padding applied
            int parentInnerWidth = flpRole.ClientSize.Width - flpRole.Padding.Horizontal;
            int moduleWidth = Math.Max(140, (parentInnerWidth - (columns - 1) * spacing) / columns);

            void UpdateModuleHeight(Panel modulePanel, Panel headerPanel, Panel permissionPanel)
            {
                modulePanel.Height = permissionPanel.Visible
                    ? headerPanel.Height + permissionPanel.Height + 8
                    : headerPanel.Height + 12;
            }

            void Reflow()
            {
                int x = flpRole.Padding.Left;
                int y = flpRole.Padding.Top;
                int col = 0;
                int rowMax = 0;

                foreach (Control ctl in flpRole.Controls)
                {
                    if (ctl is Panel panel)
                    {
                        panel.Location = new Point(x, y);
                        panel.Width = moduleWidth;

                        rowMax = Math.Max(rowMax, panel.Height);
                        col++;

                        if (col >= columns)
                        {
                            col = 0;
                            x = flpRole.Padding.Left;
                            y += rowMax + spacing;
                            rowMax = 0;
                        }
                        else x += moduleWidth + spacing;
                    }
                }
            }

            foreach (var group in groupedPermissions)
            {
                var modulePanel = new Panel
                {
                    Width = moduleWidth,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(2),
                    AutoSize = false
                };

                // Header
                var headerPanel = new Panel
                {
                    Height = 28,
                    Dock = DockStyle.Top
                };

                var btnToggle = new Button
                {
                    Text = "▶",
                    Width = 22,
                    Height = 22,
                    FlatStyle = FlatStyle.Flat
                };
                btnToggle.FlatAppearance.BorderSize = 0;
                btnToggle.Location = new Point(2, (headerPanel.Height - btnToggle.Height) / 2 + 1);

                var moduleCheckBox = new CheckBox
                {
                    Text = group.Module,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                    Location = new Point(btnToggle.Right + 6, (headerPanel.Height - 20) / 2 + 1)
                };

                headerPanel.Controls.Add(btnToggle);
                headerPanel.Controls.Add(moduleCheckBox);
                modulePanel.Controls.Add(headerPanel);

                // Permissions panel
                var permissionPanel = new Panel
                {
                    Visible = false,
                    AutoSize = true,
                    Location = new Point(18, headerPanel.Bottom + 1)
                };

                List<EventHandler> permHandlers = new List<EventHandler>();
                int yOff = 0;

                foreach (var perm in group.Permissions)
                {
                    var cb = new CheckBox
                    {
                        Text = perm.Name,
                        AutoSize = true,
                        Tag = perm,
                        Font = new Font("Segoe UI", 9.3f),
                        Location = new Point(0, yOff)
                    };

                    yOff += cb.Height + 2;

                    EventHandler eh = (s, e) =>
                    {
                        bool allChecked = permissionPanel.Controls.OfType<CheckBox>().All(c => c.Checked);
                        moduleCheckBox.CheckedChanged -= ModuleCheckHandler;
                        moduleCheckBox.Checked = allChecked;
                        moduleCheckBox.CheckedChanged += ModuleCheckHandler;
                    };

                    cb.CheckedChanged += eh;
                    permHandlers.Add(eh);
                    permissionPanel.Controls.Add(cb);
                }

                void ModuleCheckHandler(object s, EventArgs e)
                {
                    int i = 0;
                    foreach (CheckBox cb in permissionPanel.Controls.OfType<CheckBox>())
                    {
                        cb.CheckedChanged -= permHandlers[i];
                        cb.Checked = moduleCheckBox.Checked;
                        cb.CheckedChanged += permHandlers[i];
                        i++;
                    }
                }

                moduleCheckBox.CheckedChanged += ModuleCheckHandler;
                modulePanel.Controls.Add(permissionPanel);

                // Expand/Collapse event
                btnToggle.Click += (s, e) =>
                {
                    permissionPanel.Visible = !permissionPanel.Visible;
                    btnToggle.Text = permissionPanel.Visible ? "▼" : "▶";
                    UpdateModuleHeight(modulePanel, headerPanel, permissionPanel);
                    Reflow();
                };

                UpdateModuleHeight(modulePanel, headerPanel, permissionPanel);
                flpRole.Controls.Add(modulePanel);
            }
            Reflow();
        }

        private void ApplyRolePermissionsToUI()
        {
            if (_selectedRole == null) return;

            var rolePermissionNames = _selectedRole.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToHashSet();

            foreach (var modulePanel in flpRole.Controls.OfType<Panel>())
            {
                foreach (var permissionPanel in modulePanel.Controls.OfType<Panel>())
                {
                    foreach (var cb in permissionPanel.Controls.OfType<CheckBox>())
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

            foreach (var modulePanel in flpRole.Controls.OfType<Panel>())
            {
                foreach (var permissionPanel in modulePanel.Controls.OfType<Panel>())
                {
                    foreach (var cb in permissionPanel.Controls.OfType<CheckBox>())
                    {
                        if (cb.Checked && cb.Tag is Permission perm)
                            selectedPermissions.Add(perm);
                    }
                }
            }

            var newRole = new Role
            {
                Id = int.Parse(txbRoleId.Text),
                Name = txbRoleName.Text,
                IsActive = cbIsActiveRole.SelectedItem?.ToString() == "Active",
                RolePermissions = selectedPermissions
                    .Select(p => new RolePermission { PermissionId = p.Id })
                    .ToList()
            };

            await action(newRole);

            await GetListRole();
        }

        public async Task GetListRole()
        {
            var result = await RoleService.Instance.GetListRole(_pageSize, _currentPageRole, txbSearchRole.Text);

            if (result == null || !result.Items.Any())
            {
                dtgvRole.DataSource = null; 
                return;
            }

            _roles = result.Items.ToList();

            var rolesWithString = _roles.Select(r => new
            {
                r.Id,
                r.Name,
                IsActive = r.IsActive ? "Active" : "Inactive",
                Permissions = string.Join(", ", r.RolePermissions.Select(rp => rp.Permission.Name))
            }).ToList();

            rolelist.DataSource = rolesWithString;

            _totalPagesRole = result.TotalPages;

            LayoutForm.RenderPagination(
                paginatePanelRole,
                _currentPageRole,
                _totalPagesRole,
                async (newPage) =>
                {
                    _currentPageRole = newPage;
                    await GetListRole();
                }
            );
        }

        public async Task GetListPermission()
        {
            var result = await PermissionService.Instance.GetListPermission(_pageSize, _currentPagePermission, txbSearchPermission.Text);

            if (result == null || !result.Items.Any())
            {
                dtgvPermission.DataSource = null;
                return;
            }

            permissionlist.DataSource = result.Items
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Module,
                });

            dtgvPermission.DataSource = permissionlist;

            _totalPagesPermission = result.TotalPages;

            LayoutForm.RenderPagination(
                paginatePanelPermission,
                _currentPagePermission,
                _totalPagesPermission,
                async (newPage) =>
                {
                    _currentPagePermission = newPage;
                    await GetListPermission();
                }
            );
        }

        public async Task GetListAccount()
        {
            var result = await AccountService.Instance.GetListAccount(_pageSize, _currentPageAccount, txbSearchAccount.Text);

            if (result == null || !result.Items.Any())
            {
                dtgvAccount.DataSource = null;
                return;
            }

            accountlist.DataSource = result.Items
                .Select(a => new
                {
                    a.UserName,
                    a.DisplayName,
                    Role = a.Role.Name
                });

            dtgvAccount.DataSource = accountlist;

            _totalPagesAccount = result.TotalPages;

            LayoutForm.RenderPagination(
                paginatePanelAccount,
                _currentPageAccount,
                _totalPagesAccount,
                async (newPage) =>
                {
                    _currentPageAccount = newPage;
                    await GetListAccount();
                }
            );
        }

        private async Task GetListTable()
        {
            var result = await TableFoodService.Instance.LoadTableList(_pageSize, _currentPageTable, txbSearchTable.Text);

            if (result == null || !result.Items.Any())
            {
                dtgvTable.DataSource = null;
                return;
            }

            tablelist.DataSource = result.Items
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Status,
                });

            dtgvTable.DataSource = tablelist;

            _totalPagesTable = result.TotalPages;

            LayoutForm.RenderPagination(
                paginatePanelTable,
                _currentPageTable,
                _totalPagesTable,
                async (newPage) =>
                {
                    _currentPageTable = newPage;
                    await GetListTable();
                }
            );
        }

        private async Task GetListCategory()
        {
            var result = await CategoryService.Instance.GetListCategory(_pageSize, _currentPageCategory, txbSearchCategory.Text);

            if (result == null || !result.Items.Any())
            {
                dtgvCategory.DataSource = null;
                return;
            }

            categorylist.DataSource = result.Items.
                Select(c => new
                {
                    c.Id,
                    c.Name,
                });

            dtgvCategory.DataSource = categorylist;

            _totalPagesCategory = result.TotalPages;

            LayoutForm.RenderPagination(
                paginatePanelCategory,
                _currentPageCategory,
                _totalPagesCategory,
                async (newPage) =>
                {
                    _currentPageCategory = newPage;
                    await GetListCategory();
                }
            );
        }

        private async Task GetListFood()
        {
            var result = await FoodService.Instance.GetListFood(_pageSize, _currentPageFood, txbSearchFood.Text);

            if (result == null || !result.Items.Any())
            {
                dtgvFood.DataSource = null;
                return;
            }

            foodlist.DataSource = result.Items;

            dtgvFood.DataSource = foodlist;

            _totalPagesFood = result.TotalPages;

            LayoutForm.RenderPagination(
                paginatePanelFood,
                _currentPageFood,
                _totalPagesFood,
                async (newPage) =>
                {
                    _currentPageFood = newPage;
                    await GetListFood();
                }
            );
        }
   
        private void LoadTableStatus()
        {
            cbTableStatus.DataSource = new List<string> { "Empty", "Reserved", "Merged" };
        }

        private async Task LoadRoleToComboBox()
        {
            cbIsActiveRole.DataSource = new List<string> { "Active", "Inactive" };

            cbAccountRole.DataSource = (await RoleService.Instance.GetListRole()).Items
                .Where(r => r.IsActive)
                .ToList();

            cbAccountRole.DisplayMember = "Name";
        }
        #endregion

        #region Events
        public async void LoadData(object sender, EventArgs e)
        {
            var billTask = LoadBillListDefault();
            var permissionTask = LoadGroupPermission();

            await Task.WhenAll(billTask, permissionTask);

            await LoadRoleToComboBox();

            LoadTableStatus();
        }

        private async void btnViewBill_Click(object sender, EventArgs e)
        {
            await LoadBillList(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private async void btnViewFood_Click(object sender, EventArgs e)
        {
            await GetListFood();

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

            await GetListFood();

            await _tableManager.LoadCategory();
        }

        private async void btnUpdateFood_Click(object sender, EventArgs e)
        {
            if (foodlist.Count == 0) return;

            FoodDTO food = new FoodDTO(int.Parse(txbFoodId.Text), txbFoodName.Text, cbFoodCategory.Text, (float)nmFoodPrice.Value);

            await FoodService.Instance.UpdateFood(food);

            await GetListFood();

            await _tableManager.LoadCategory();
        }

        private async void btnDeleteFood_Click(object sender, EventArgs e)
        {
            if(foodlist.Count == 0) return;

            bool confirm = Confirmation.ShowConfirm("Delete Confirmation", 
                "Are you sure you want to delete this food?");

            if (!confirm) return;

            List<TableFood> tables = await FoodService.Instance.DeleteFood(int.Parse(txbFoodId.Text));

            await GetListFood();

            await _tableManager.LoadCategory();  
            
            foreach (TableFood tableFood in tables)
            {
                _tableManager.UpdateTableButtonColor(tableFood);
            }
        }

        private async void btnSearchFood_Click(object sender, EventArgs e)
        {
            if (foodlist.Count > 0) await GetListFood();
        }

        private async void btnViewRole_Click(object sender, EventArgs e)
        {
            await GetListRole();

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
            if (rolelist.Count > 0)
            {
                await InsertOrUpdateRole(RoleService.Instance.InsertRole);
                await LoadRoleToComboBox();
            }
        }

        private async void btnUpdateRole_Click(object sender, EventArgs e)
        {
            if (rolelist.Count > 0)
            {
                await InsertOrUpdateRole(RoleService.Instance.UpdateRole);
                await LoadRoleToComboBox();
            }
        }

        private async void btnDeleteRole_Click(object sender, EventArgs e)
        {
            if (rolelist.Count == 0) return;

            bool confirm = Confirmation.ShowConfirm("Delete Confirmation",
                 "Are you sure you want to delete this role?");

            if (!confirm) return;

            await RoleService.Instance.DeleteRole(int.Parse(txbRoleId.Text));

            await GetListRole();

            await LoadRoleToComboBox();
        }

        private async void btnSearchRole_Click(object sender, EventArgs e)
        {
            if(rolelist.Count > 0) await GetListRole();
        }

        private async void btnViewPermission_Click(object sender, EventArgs e)
        {
            await GetListPermission();

            txbPermissionId.DataBindings.Clear();
            txbPermissionName.DataBindings.Clear();
            txbPermissionModule.DataBindings.Clear();

            txbPermissionId.DataBindings.Add("Text", permissionlist, "Id", true, DataSourceUpdateMode.Never);
            txbPermissionName.DataBindings.Add("Text", permissionlist, "Name", true, DataSourceUpdateMode.Never);
            txbPermissionModule.DataBindings.Add("Text", permissionlist, "Module", true, DataSourceUpdateMode.Never);
        }

        private async void btnSearchPermission_Click(object sender, EventArgs e)
        {
            if(permissionlist.Count > 0) await GetListPermission();
        }

        private async void btnAddPermission_Click(object sender, EventArgs e)
        {
            if (permissionlist.Count == 0) return;

            Permission permission = new Permission
            {
                Name = txbPermissionName.Text,
                Module = txbPermissionModule.Text,
            };

            await PermissionService.Instance.InsertPermission(permission);

            await GetListPermission();

            await LoadGroupPermission();
        }

        private async void btnUpdatePermission_Click(object sender, EventArgs e)
        {
            if (permissionlist.Count == 0) return;

            Permission permission = new Permission
            {
                Id = int.Parse(txbPermissionId.Text),
                Name = txbPermissionName.Text,
                Module = txbPermissionModule.Text,
            };

            await PermissionService.Instance.UpdatePermission(permission);

            await GetListPermission();

            await LoadGroupPermission();
        }

        private async void btnDeletePermission_Click(object sender, EventArgs e)
        {
            if (permissionlist.Count == 0) return;

            bool confirm = Confirmation.ShowConfirm("Delete Confirmation",
                "Are you sure you want to delete this permission?");

            if (!confirm) return;

            await PermissionService.Instance.DeletePermission(int.Parse(txbPermissionId.Text));

            await GetListPermission();

            await LoadGroupPermission();
        }

        private async void btnViewAccount_Click(object sender, EventArgs e)
        {
            await GetListAccount();

            txbAccountUsername.DataBindings.Clear();
            txbAccountDisplayname.DataBindings.Clear();
            cbAccountRole.DataBindings.Clear();

            txbAccountUsername.DataBindings.Add("Text", accountlist, "UserName", true, DataSourceUpdateMode.Never);
            txbAccountDisplayname.DataBindings.Add("Text", accountlist, "DisplayName", true, DataSourceUpdateMode.Never);
            cbAccountRole.DataBindings.Add("Text", accountlist, "Role", true, DataSourceUpdateMode.Never);
        }

        private async void btnSearchAccount_Click(object sender, EventArgs e)
        {
            if(accountlist.Count > 0) await GetListAccount();
        }

        private async void btnAddAccount_Click(object sender, EventArgs e)
        {
            if (accountlist.Count == 0) return;


            Account newAccount = new Account
            {
                UserName = txbAccountDisplayname.Text.Trim().ToLower().Replace(" ", "_"),
                DisplayName = txbAccountDisplayname.Text,
                PassWord = BCrypt.Net.BCrypt.HashPassword("123456"),
                IdRole = (cbAccountRole.SelectedItem as Role).Id,
                Image = string.IsNullOrEmpty(_filename) ? "default.png" : _filename,
            };

            await AccountService.Instance.InsertAccount(newAccount);

            await GetListAccount();
        }

        private async void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            if (accountlist.Count == 0) return;

            Account newAccount = new Account
            {
                UserName = txbAccountUsername.Text,
                DisplayName = txbAccountDisplayname.Text,
                IdRole = (cbAccountRole.SelectedItem as Role).Id,
                Image = string.IsNullOrEmpty(_filename) ? "default.png" : _filename,
            };

            var updateAccount = await AccountService.Instance.UpdateAccount(newAccount);

            if(updateAccount.UserName == AccountService.Instance.User.UserName)
            {
                AccountService.Instance.User = updateAccount;

                _tableManager.accountToolStripDropdown.Text = $"Account ({updateAccount.DisplayName})";
            }

            await GetListAccount();
        }

        private async void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            if (accountlist.Count == 0) return;

            bool confirm = Confirmation.ShowConfirm("Delete Confirmation",
                "Are you sure you want to delete this account?");

            if (!confirm) return;

            await AccountService.Instance.DeleteAccount(txbAccountUsername.Text);

            await GetListAccount();
        }

        private async void btnResetAccount_Click(object sender, EventArgs e)
        {
            if (accountlist.Count == 0) return;

            bool confirm = Confirmation.ShowConfirm("Delete Confirmation",
                "Are you sure you want to reset this account?");

            if (!confirm) return;

            await AccountService.Instance.ResetAccountPassword(txbAccountUsername.Text);
        }

        private async void btnViewTable_Click(object sender, EventArgs e)
        {
            await GetListTable();    

            txbTableId.DataBindings.Clear();
            txbTableName.DataBindings.Clear();
            cbTableStatus.DataBindings.Clear();

            txbTableId.DataBindings.Add("Text", tablelist, "Id", true, DataSourceUpdateMode.Never);
            txbTableName.DataBindings.Add("Text", tablelist, "Name", true, DataSourceUpdateMode.Never);
            cbTableStatus.DataBindings.Add("Text", tablelist, "Status", true, DataSourceUpdateMode.Never);
        }

        private async void btnSearchTable_Click(object sender, EventArgs e)
        {
            if(tablelist.Count > 0) await GetListTable();
        }

        private async void btnAddTable_Click(object sender, EventArgs e)
        {
            if (tablelist.Count == 0) return;

            TableFood tableFood = new TableFood
            {
                Name = txbTableName.Text,
                Status = cbTableStatus.Text,
            };

            await TableFoodService.Instance.InsertTable(tableFood);

            await GetListTable();

            await _tableManager.LoadTableFood();
        }

        private async void btnUpdateTable_Click(object sender, EventArgs e)
        {
            if (tablelist.Count == 0) return;

            TableFood tableFood = new TableFood
            {
                Id = int.Parse(txbTableId.Text),   
                Name = txbTableName.Text,
                Status = cbTableStatus.Text,
            };

            await TableFoodService.Instance.UpdateTable(tableFood);

            await GetListTable();

            await _tableManager.LoadTableFood();
        }

        private async void btnDeleteTable_Click(object sender, EventArgs e)
        {
            if (tablelist.Count == 0) return;

            bool confirm = Confirmation.ShowConfirm("Delete Confirmation",
                "Are you sure you want to delete this table?");

            if (!confirm) return;

            await TableFoodService.Instance.DeleteTable(int.Parse(txbTableId.Text));

            await GetListTable();

            await _tableManager.LoadTableFood();
        }

        private async void btnViewCategory_Click(object sender, EventArgs e)
        {
            await GetListCategory();

            txbCategoryId.DataBindings.Clear();
            txbCategoryName.DataBindings.Clear();

            txbCategoryId.DataBindings.Add("Text", categorylist, "Id", true, DataSourceUpdateMode.Never);
            txbCategoryName.DataBindings.Add("Text", categorylist, "Name", true, DataSourceUpdateMode.Never);
        }

        private async void btnSearchCategory_Click(object sender, EventArgs e)
        {
            if(categorylist.Count > 0) await GetListCategory();
        }

        private async void btnAddCategory_Click(object sender, EventArgs e)
        {
            if (categorylist.Count == 0) return;

            FoodCategory foodCategory = new FoodCategory() { Name = txbCategoryName.Text };

            await CategoryService.Instance.InsertCategory(foodCategory);

            await GetListCategory();

            await _tableManager.LoadCategory();    
        }

        private async void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            if (categorylist.Count == 0) return;

            FoodCategory foodCategory = new FoodCategory() 
            { 
                Id = int.Parse(txbCategoryId.Text),
                Name = txbCategoryName.Text 
            };

            await CategoryService.Instance.UpdateCategory(foodCategory);

            await GetListCategory();

            await _tableManager.LoadCategory();
        }

        private async void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (categorylist.Count == 0) return;

            bool confirm = Confirmation.ShowConfirm("Delete Confirmation",
                "Are you sure you want to delete this category?");

            if (!confirm) return;

            await CategoryService.Instance.DeleteCategory(int.Parse(txbCategoryId.Text));

            await GetListCategory();

            await _tableManager.LoadCategory();
        }

        private async void dtgvAccount_SelectionChanged(object sender, EventArgs e)
        {
            if (dtgvAccount.CurrentRow == null) return;

            var data = dtgvAccount.CurrentRow.DataBoundItem;
            if (data == null) return;

            var userName = data.GetType().GetProperty("UserName")?.GetValue(data)?.ToString();
            if (string.IsNullOrEmpty(userName)) return;

            var currentAccount = await AccountService.Instance.GetSingleAccount(userName);
            if (currentAccount == null) return;

            ImageService.Instance.LoadAccountImageToButton(currentAccount, btnUploadAccount);

            _filename = currentAccount.Image;
        }

        private void btnUploadAccount_Click(object sender, EventArgs e)
        {
            string filename = ImageService.Instance.UploadAccountImageDialog(adminAccountFileDialog);

            if (!string.IsNullOrEmpty(filename))
            {
                _filename = filename;
                ImageService.Instance.UpdateButtonImage(filename, btnUploadAccount);
            }
        }
        #endregion
    }
}
