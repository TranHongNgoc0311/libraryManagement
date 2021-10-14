using LibraMaster_ver2._0.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0
{
    public partial class LayoutForm : Form
    {
        public LayoutForm()
        {
            InitializeComponent();
            userItem.Text = UserLoginData.Userdata.email + "\n" + UserLoginData.Userdata.name;
            HomeForm form = new HomeForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();

            if(UserLoginData.Userdata.level != 0)
            {
                thủThưToolStripMenuItem.Visible = false;
            }
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Email = string.Empty;
            Properties.Settings.Default.Password = string.Empty;
            Properties.Settings.Default.Save();
            this.Hide();
            LoginForm form = new LoginForm();
            form.Show();
        }

        private void đóngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show(this, "Bạn chắc chắn muốn đóng ứng dụng?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(rs == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void mụcLụcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ActiveMdiChild.Close();
            frmBook.BookMain form = new frmBook.BookMain();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();
        }

        private void LayoutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void tìmSáchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBook.SearchBook search = new frmBook.SearchBook();
            search.ShowDialog();

            List<Book> books = search.searchRs();
            if (books != null)
            {
                this.ActiveMdiChild.Close();
                frmBook.BookMain form = new frmBook.BookMain(search.searchRs());
                form.MdiParent = this;
                form.Dock = DockStyle.Fill;
                form.ControlBox = false;
                form.Show();
            }
        }

        private void tácGiảVàThểLoạiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ActiveMdiChild.Close();
            frmBook.AuthorCategoryForm form = new frmBook.AuthorCategoryForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            this.ActiveMdiChild.Close();
            HomeForm form = new HomeForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();

        }

        private void thủThưToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ActiveMdiChild.Close();
            AccountsForm form = new AccountsForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();
        }

        private void danhSáchĐọcGiảToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ActiveMdiChild.Close();
            frmMember.MembersForm form = new frmMember.MembersForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.ActiveMdiChild.Close();
            frmBorrow.BorrowForm form = new frmBorrow.BorrowForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();
        }

        private void traCứuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMember.SearchForm search = new frmMember.SearchForm();
            search.ShowDialog();

            Member m = search.searchRs();
            if (m != null)
            {
                frmMember.DetailForm form = new frmMember.DetailForm(m.id);
                form.ShowDialog();
            }
        }

        private void phiếuMượnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void thốngKêPhiếuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ActiveMdiChild.Close();
            frmBorrow.BorrowListForm form = new frmBorrow.BorrowListForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();
        }
    }
}
