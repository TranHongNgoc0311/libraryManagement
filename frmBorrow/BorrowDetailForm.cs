using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0.frmBorrow
{
    public partial class BorrowDetailForm : Form
    {
        string id;
        ProjectLibraryManagementDataContext db;
        public BorrowDetailForm()
        {
            InitializeComponent();
        }
        public BorrowDetailForm(string id)
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            this.id = id;
            var br = db.Borrows.SingleOrDefault(x => x.id == id);
            lbBrId.Text = id;
            LbAddress.Text = br.Member.address;
            LbBrDate.Text = br.borrowDate.Value.ToString("dd/MM/yyyy");
            LbEmail.Text = br.Member.email;
            LbMemId.Text = br.memberId.ToString();
            LbPhone.Text = br.Member.phone;
            LbReturnDate.Text = br.returnDateIssua.Value.ToString("dd/MM/yyyy");
            LbStatus.Text = (br.status == true) ? "Hoàn thành" : "Chưa hoàn thành";
            
            if (br.Member.image != null)
            {
                MemoryStream ms = new MemoryStream(br.Member.image.ToArray());
                PbImage.Image = Image.FromStream(ms);
            }

            showDetail();
        }

        public void showDetail()
        {
            var br = db.Borrows.SingleOrDefault(x => x.id == id);
            DgvBorrowsDetail.Rows.Clear();
            foreach (var bd in br.BorrowDetails)
            {
                DgvBorrowsDetail.Rows.Add(
                    bd.bookId,
                    bd.Book.name,
                    (bd.returnedDate == null) ? "Chưa trả" : bd.returnedDate.Value.ToString("dd/MM/yyyy"),
                    bd.countDaysLate,
                    bd.returnStatus,
                    bd.fee,
                    (bd.status == true) ? "Đã trả" : "Chưa trả"
                );
            }
        }

        private void trảSáchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(DgvBorrowsDetail.Rows.Count > 0)
            {
                var row = DgvBorrowsDetail.CurrentRow;
                var detail = db.BorrowDetails.SingleOrDefault(
                    x => x.bookId.ToString() == row.Cells[0].Value.ToString()
                    && x.borrowId == lbBrId.Text);
                if(row.Cells[6].Value.ToString().Equals("Chưa trả")) {
                    ReturnBorrowForm form = new ReturnBorrowForm(detail);
                    form.ShowDialog();

                    var br = db.Borrows.SingleOrDefault(x => x.id == id);
                    DgvBorrowsDetail.Rows.Clear();
                    foreach (var bd in br.BorrowDetails)
                    {
                        DgvBorrowsDetail.Rows.Add(
                            bd.bookId,
                            bd.Book.name,
                            (bd.returnedDate == null) ? "Chưa trả" : bd.returnedDate.Value.ToString("dd/MM/yyyy"),
                            bd.countDaysLate,
                            bd.returnStatus,
                            bd.fee,
                            (bd.status == true) ? "Đã trả" : "Chưa trả"
                        );
                    }
                }
                else
                {
                    MessageBox.Show("Đầu sách đã được trả rồi.","Thông báo");
                }
            }
        }
    }
}
