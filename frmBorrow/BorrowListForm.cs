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
    public partial class BorrowListForm : Form
    {
        ProjectLibraryManagementDataContext db;
        public BorrowListForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            showList();
        }

        public void showList()
        {
            DgvBorrows.Rows.Clear();
            var borrows = db.Borrows.ToList();
            foreach (var b in borrows)
            {
                DgvBorrows.Rows.Add(
                    b.id, 
                    b.memberId, 
                    b.borrowDate.Value.ToString("dd/MM/yyyy"), 
                    b.returnDateIssua.Value.ToString("dd/MM/yyyy"), 
                    b.totalBook,
                    b.BorrowDetails.Where(x => x.status == true).Count() * 100 / (b.totalBook) + "%", 
                    (b.status == true) ? "Hoàn thành" : "Chưa hoàn thành"
                );
            }
        }

        public void delBorrow()
        {
            DialogResult rs = MessageBox.Show("Bạn chắc chắn muốn xóa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.Yes)
            {
                bool check = true;
                try
                {
                    var rows = DgvBorrows.CurrentRow;
                    string id = rows.Cells[0].Value.ToString();
                    var borrowDetail = db.BorrowDetails.Where(x => x.borrowId == id).ToList();
                    foreach (var bd in borrowDetail)
                    {
                        bd.Book.status = true;
                    }
                    db.BorrowDetails.DeleteAllOnSubmit(borrowDetail);
                    db.Borrows.DeleteOnSubmit(db.Borrows.SingleOrDefault(x => x.id == id));
                    db.SubmitChanges();
                }
                catch (Exception)
                {
                    check = false;
                }
                finally
                {
                    if (check)
                    {
                        MessageBox.Show(this, "Xóa thành công.", "Thông báo");
                        showList();
                    }
                    else
                    {
                        MessageBox.Show(this, "Xóa thất bại.", "Thông báo");
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            delBorrow();
        }

        public void viewDetail()
        {
            if (DgvBorrows.Rows.Count > 0)
            {
                var row = DgvBorrows.CurrentRow;
                string id = row.Cells[0].Value.ToString();
                BorrowDetailForm form = new BorrowDetailForm(id);
                form.ShowDialog();
                showList();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            viewDetail();
        }

        private void chiTiếtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewDetail();
        }

        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delBorrow();
        }
    }
}
