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

namespace LibraMaster_ver2._0.frmMember
{
    public partial class DetailForm : Form
    {
        ProjectLibraryManagementDataContext db;
        int id;
        public DetailForm(int id)
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            this.id = id;
            loadInfo();
            showBorrow();
        }

        public void loadInfo()
        {
            var info = db.Members.SingleOrDefault(x => x.id == id);
            LbCreated.Text = info.created.ToString();
            lbId.Text = info.id.ToString();
            LbFname.Text = info.firstName;
            LbLname.Text = info.lastName;
            LbPhone.Text = info.phone;
            LbEmail.Text = info.email;
            LbAddress.Text = info.address;
            LbBirth.Text = info.birthday.Value.ToString("dd/MM/yyyy");
            if (info.gender == true)
            {
                LbGender.Text = "Nam";
            }
            else
            {
                LbGender.Text = "Nữ";
            }
            if (info.image != null)
            {
                MemoryStream ms = new MemoryStream(info.image.ToArray());
                PbImage.Image = Image.FromStream(ms);
            }
        }

        public void showBorrow()
        {
            DgvBorrows.Rows.Clear();
            var br = db.Borrows.Where(x => x.memberId == id).ToList();
            foreach (var x in br)
            {
                DgvBorrows.Rows.Add(
                    x.id, x.totalBook, 
                    x.borrowDate.Value.ToString("dd/MM/yyyy"), 
                    x.returnDateIssua.Value.ToString("dd/MM/yyyy"),   
                    (db.BorrowDetails.Where(bd => bd.borrowId == x.id && bd.status == true).Count())/(x.totalBook)*100+"%", 
                    (x.status == true)?"Hoàn thành":"Chưa hoàn thành");
            }
        }

        private void DgvBorrows_DoubleClick(object sender, EventArgs e)
        {
            if (DgvBorrows.Rows.Count > 0)
            {
                var row = DgvBorrows.CurrentRow;
                string id = row.Cells[0].Value.ToString();
                frmBorrow.BorrowDetailForm form = new frmBorrow.BorrowDetailForm(id);
                form.ShowDialog();
            }
        }

        private void chiTiếtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(DgvBorrows.Rows.Count > 0)
            {
                var row = DgvBorrows.CurrentRow;
                frmBorrow.BorrowDetailForm form = new frmBorrow.BorrowDetailForm(row.Cells[0].Value.ToString());
                form.ShowDialog();
            }
        }
    }
}
