using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0.frmMember
{
    public partial class MembersForm : Form
    {
        ProjectLibraryManagementDataContext db;
        public MembersForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();

            showMembers();
        }


        public void showMembers()
        {
            DgvMem.Rows.Clear();
            var members = db.Members.ToList();
            foreach (var x in members)
            {
                DgvMem.Rows.Add(x.id, x.firstName + " " + x.lastName, (x.gender == true) ? "Nam" : "Nữ", x.birthday.Value.ToString("dd/MM/yyyy"), x.phone, x.email, x.address, x.created);
            }
        }

        public void delMember()
        {
            DialogResult rs = MessageBox.Show("Bạn chắc chắn muốn xóa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(rs == DialogResult.Yes)
            {
                bool check = true;
                try
                {
                    var rows = DgvMem.CurrentRow;
                    int id = int.Parse(rows.Cells[0].Value.ToString());
                    var borrows = db.Borrows.Where(x => x.memberId == id).ToList();
                    foreach (var br in borrows)
                    {
                        var details = db.BorrowDetails.Where(x => x.borrowId == br.id).ToList();
                        foreach (var bd in details)
                        {
                            db.Books.SingleOrDefault(x => x.id == bd.bookId).status = true;
                        }
                        db.BorrowDetails.DeleteAllOnSubmit(details);
                    }
                    db.Borrows.DeleteAllOnSubmit(borrows);
                    db.Members.DeleteOnSubmit(db.Members.SingleOrDefault(x => x.id == id));
                    db.SubmitChanges();
                    showMembers();
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
                    }
                    else
                    {
                        MessageBox.Show(this, "Xóa thất bại.", "Thông báo");
                    }
                }
            }
        }

        public void updateMember()
        {
            if(DgvMem.Rows.Count > 0)
            {
                int id = int.Parse(DgvMem.CurrentRow.Cells[0].Value.ToString());
                DataEditForm form = new DataEditForm(id);
                form.ShowDialog();
            }
            showMembers();
        }

        public void detailMember()
        {
            if (DgvMem.Rows.Count > 0)
            {
                int id = int.Parse(DgvMem.CurrentRow.Cells[0].Value.ToString());
                DetailForm form = new DetailForm(id);
                form.ShowDialog();
            }
        }
        private void cậpNhậtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateMember();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            delMember();
        }

        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delMember();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DataEditForm form = new DataEditForm();
            form.ShowDialog();
            showMembers();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            updateMember();
        }

        private void chiTiếtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailMember();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            detailMember();
        }
    }
}
