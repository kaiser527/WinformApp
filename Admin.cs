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
        public Admin()
        {
            InitializeComponent();
            Load += LoadBillListDefault;
        }

        #region methods
        private async Task LoadBillList(System.DateTime checkIn, System.DateTime checkOut)
        {
            IEnumerable<BillDTO> bills = await BillService.Instance.GetCheckedBillByDate(checkIn, checkOut);
            dtgvBill.DataSource = bills;
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

            await LoadBillList(dtpkFromDate.Value, dtpkToDate.Value);
        }
        #endregion
    }
}
