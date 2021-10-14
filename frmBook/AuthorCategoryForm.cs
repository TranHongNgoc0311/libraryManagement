using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0.frmBook
{
    public partial class AuthorCategoryForm : Form
    {
        ProjectLibraryManagementDataContext db;
        bool catEdit = false;
        public AuthorCategoryForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            showAuthors();
            showCat();
        }
        public void showCat()
        {
            DgvCat.Rows.Clear();
            var cats = db.Categories.ToList();
            foreach (var c in cats)
            {
                DgvCat.Rows.Add(c.id, c.name, (c.status == true) ? "Hiện" : "Ẩn");
            }
        }

        public void getCat()
        {
            var row = DgvCat.CurrentRow;
            Category cat = db.Categories.SingleOrDefault(x => x.id.ToString() == row.Cells[0].Value.ToString());
            txtId.Text = cat.id.ToString();
            txtName.Text = cat.name;
            if(cat.status == true)
            {
                RbHien.Checked = true;
            }
            else
            {
                RbAn.Checked = true;
            }
        }

        public void showAuthors()
        {
            DgvAuthors.Rows.Clear();
            var authors = db.Authors.ToList();
            foreach (var a in authors)
            {
                DgvAuthors.Rows.Add(a.id,a.trueName,a.name,a.birthday.Value.ToString("dd/MM/yyyy"), (a.gender == true)?"Nam":"Nữ",db.AuthorsBooks.Where(x => x.AuthorId == a.id).Count());
            }
        }

        private void txtSearchA_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearchA.Text;
            DgvAuthors.Rows.Clear();
            var authors = db.Authors.Where(x => x.name.Contains(keyword) || x.trueName.Contains(keyword)).ToList();
            foreach(var a in authors)
            {
                DgvAuthors.Rows.Add(a.id, a.trueName, a.name, a.birthday.Value.ToString("dd/MM/yyyy"), (a.gender == true) ? "Nam" : "Nữ", db.AuthorsBooks.Where(x => x.AuthorId == a.id).Count());
            }
        }

        public void updateDataAuthor()
        {
            var rows = DgvAuthors.CurrentRow;
            var au = db.Authors.SingleOrDefault(x => x.id.ToString() == rows.Cells[0].Value.ToString());
            AuthorDataForm form = new AuthorDataForm(au);
            form.ShowDialog();
            showAuthors();
        }

        public void detailDataAuthor()
        {
            var rows = DgvAuthors.CurrentRow;
            var books = db.AuthorsBooks.Where(x => x.AuthorId.ToString() == rows.Cells[0].Value.ToString()).Select(x => x.Book).ToList();
            BookMain form = new BookMain(books);
            var parent = this.MdiParent;
            parent.ActiveMdiChild.Close();
            form.MdiParent = parent;
            form.Dock = DockStyle.Fill;
            form.ControlBox = false;
            form.Show();
        }

        public void deleteDataAuthor()
        {
            DialogResult rs = MessageBox.Show("Bạn chắc chắn muốn xóa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(rs == DialogResult.Yes)
            {
                bool check = true;
                try
                {
                    var rows = DgvAuthors.CurrentRow;
                    db.AuthorsBooks.DeleteAllOnSubmit(db.AuthorsBooks.Where(x => x.AuthorId.ToString() == rows.Cells[0].Value.ToString()));
                    db.Authors.DeleteOnSubmit(db.Authors.SingleOrDefault(x => x.id.ToString() == rows.Cells[0].Value.ToString()));
                    db.SubmitChanges();
                    showAuthors();
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AuthorDataForm form = new AuthorDataForm();
            form.ShowDialog();
            showAuthors();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            detailDataAuthor();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            updateDataAuthor();
        }

        private void cậpNhậtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateDataAuthor();
        }

        private void chiTiếtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailDataAuthor();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            deleteDataAuthor();
        }

        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteDataAuthor();
        }

        private void DgvCat_Click(object sender, EventArgs e)
        {
            if(DgvCat.Rows.Count > 0)
            {
                getCat();
                catEdit = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtId.Text = string.Empty;
            txtName.Text = string.Empty;
            catEdit = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            getCat();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show(this, "Bạn chắc chắn muốn xóa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.Yes)
            {
                bool check = true;
                try
                {
                    int id = int.Parse(txtId.Text);
                    db.Books.DeleteAllOnSubmit(db.Books.Where(x => x.catID == id).ToList());
                    db.Categories.DeleteOnSubmit(db.Categories.SingleOrDefault(x => x.id == id));
                    db.SubmitChanges();
                    showCat();
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

        private void button2_Click(object sender, EventArgs e)
        {
            string errors = "";

            if (txtName.Text.Length == 0)
            {
                errors += "\n- Tên thể loại không được để trống.";
            }
            else if (db.Categories.SingleOrDefault(x => x.name.Equals(txtName.Text)) != null )
            {
                if (catEdit)
                {
                    var check = db.Categories.SingleOrDefault(x => x.id == int.Parse(txtId.Text));
                    if (!check.name.Equals(txtName.Text))
                    {
                        errors += "\n- Tên thể loại đã được sử dụng.";
                    }
                }
                else
                {
                    errors += "\n- Tên thể loại đã được sử dụng.";
                }
            }

            if (errors == string.Empty)
            {
                Category cat = new Category();
                cat.name = txtName.Text;
                if (RbHien.Checked)
                {
                    cat.status = true;
                }
                else
                {
                    cat.status = false;
                }

                if (!catEdit)
                {
                    bool rs = true;
                    try
                    {
                        db.Categories.InsertOnSubmit(cat);
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
                            MessageBox.Show(this, "Thể loại " + cat.name + " Đã được thêm vào.", "Thông báo");
                        }
                        else
                        {
                            MessageBox.Show(this, "Lỗi! không thể thêm.", "Thông báo");
                        }
                    }
                }
                else
                {
                    var check = db.Categories.SingleOrDefault(x => x.id == int.Parse(txtId.Text));
                    if (check != null)
                    {
                        bool rs = true;
                        try
                        {
                            check.name = txtName.Text;
                            if (RbHien.Checked)
                            {
                                check.status = true;
                            }
                            else
                            {
                                check.status = false;
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
                                MessageBox.Show(this, "Thể loại " + check.name + " Đã được Cập nhật.", "Thông báo");
                            }
                            else
                            {
                                MessageBox.Show(this, "Lỗi! Cập nhật thất bại.", "Thông báo");
                            }
                        }
                    }
                }
                showCat();
            }
            else
            {
                MessageBox.Show(this, errors, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
