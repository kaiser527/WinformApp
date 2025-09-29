using System.Collections.Generic;
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
        public Admin(TableManager tableManager)
        {
            InitializeComponent();
            Load += LoadBillListDefault;
            _tableManager = tableManager;
        }

        #region methods
        private async Task LoadBillList(System.DateTime checkIn, System.DateTime checkOut)
        {
            IEnumerable<BillDTO> bills = await BillService.Instance.GetCheckedBillByDate(checkIn, checkOut);
            dtgvBill.DataSource = bills;
        }

        private async Task LoadCategoryList()
        {
            cbFoodCategory.DataSource = await CategoryService.Instance.GetListCategory();
            cbFoodCategory.DisplayMember = "Name";
        }
        #endregion

        #region events
        private async void btnViewBill_Click(object sender, System.EventArgs e)
        {
            await LoadBillList(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private async void LoadBillListDefault(object sender, System.EventArgs e)
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

        private async void btnViewFood_Click(object sender, System.EventArgs e)
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

        private async void btnAddFood_Click(object sender, System.EventArgs e)
        {
            FoodDTO food = new FoodDTO(0, txbFoodName.Text, cbFoodCategory.Text, (float)nmFoodPrice.Value);

            await FoodService.Instance.InsertFood(food);

            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);

            await _tableManager.LoadCategory();
        }

        private async void btnUpdateFood_Click(object sender, System.EventArgs e)
        {
            FoodDTO food = new FoodDTO(int.Parse(txbFoodId.Text), txbFoodName.Text, cbFoodCategory.Text, (float)nmFoodPrice.Value);

            await FoodService.Instance.UpdateFood(food);

            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);

            await _tableManager.LoadCategory();
        }

        private async void btnDeleteFood_Click(object sender, System.EventArgs e)
        {
            List<TableFood> tables = await FoodService.Instance.DeleteFood(int.Parse(txbFoodId.Text));

            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);

            await _tableManager.LoadCategory();  
            
            foreach (TableFood tableFood in tables)
            {
                _tableManager.UpdateTableButtonColor(tableFood);
            }
        }

        private async void btnSearchFood_Click(object sender, System.EventArgs e)
        {
            foodlist.DataSource = await FoodService.Instance.GetListFood(txbSearchFood.Text);
        }
        #endregion
    }
}
