using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0.frmBorrow
{
    public partial class ReturnBorrowForm : Form
    {

        ProjectLibraryManagementDataContext db;
        BorrowDetail detail = new BorrowDetail();
        public ReturnBorrowForm()
        {
            InitializeComponent();
        }

        public ReturnBorrowForm(BorrowDetail detail)
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            this.detail = detail;
            lbBrId.Text = detail.borrowId;
            LbBookId.Text = detail.bookId.ToString();
            LbBookName.Text = detail.Book.name;
            LbBrDate.Text = detail.Borrow.borrowDate.Value.ToString("dd/MM/yyyy");
            LbReturnDate.Text = detail.Borrow.returnDateIssua.Value.ToString("dd/MM/yyyy");
            LbReturn.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TimeSpan t = (TimeSpan)(DateTime.Now - detail.Borrow.returnDateIssua);
            LbDayLate.Text = (t.TotalDays <0)?"0": Convert.ToInt32(t.TotalDays).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool rs = true;
            try
            {
                var info = db.BorrowDetails.SingleOrDefault(x => x.borrowId == detail.borrowId && x.bookId == detail.bookId);
                info.returnStatus = txtBookStatus.Text;
                info.fee = double.Parse((txtFee.Text == string.Empty) ? "0" : txtFee.Text);
                info.countDaysLate = int.Parse(LbDayLate.Text);
                info.returnedDate = DateTime.Now;
                info.status = true;
                info.Borrow.totalFee = db.BorrowDetails.Where(x => x.borrowId == info.borrowId).Sum(x => x.fee);
                info.Book.status = true;

                if (info.Borrow.BorrowDetails.Count() == info.Borrow.BorrowDetails.Count(x => x.status == true))
                {
                    info.Borrow.status = true;
                }

                db.SubmitChanges();
            }
            catch (Exception)
            {
                rs = false;
            }
            finally
            {
                if (rs)
                {
                    MessageBox.Show(this, "Đầu sách " + detail.Book.name + " của mã phiếu "+detail.borrowId+" đã được trả.", "Thông báo");
                }
                else
                {
                    MessageBox.Show(this, "Lỗi! trả sách thất bại.", "Thông báo");
                }
            }
        }
    }
}
