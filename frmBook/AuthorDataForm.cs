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
    public partial class AuthorDataForm : Form
    {
        ProjectLibraryManagementDataContext db;
        bool edit = false;
        public AuthorDataForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            LbTitle.Text = "Thêm tác giả";
            LbId.Visible = LbCreated.Visible = label2.Visible = label8.Visible = false;
        }
        public AuthorDataForm(Author author)
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            LbId.Text = author.id.ToString();
            LbCreated.Text = author.created.ToString();
            txtName.Text = author.name;
            txtTrueName.Text = author.trueName;
            DtpBirth.Value = (DateTime)author.birthday;
            if (author.gender == true)
            {
                RbNam.Checked = true;
            }
            else
            {
                RbNu.Checked = true;
            }
            edit = true;
            LbTitle.Text = "Cập nhật tác giả";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string errors = "";

            if(txtName.Text == string.Empty)
            {
                errors += "\n - Bút danh của tác giả không được để trống.";
            }
            else if(db.Authors.SingleOrDefault(x => x.name.Equals(txtName.Text)) != null)
            {
                if (edit)
                {
                    var check = db.Authors.SingleOrDefault(x => x.id.ToString() == LbId.Text);
                    if (!check.name.Equals(txtName.Text))
                    {
                        errors += "\n - Bút danh này đã có rồi.";
                    }
                }
                else
                {
                    errors += "\n - Bút danh này đã có rồi.";
                }
            }

            if(txtTrueName.Text == string.Empty)
            {
                errors += "\n - Tên tác giả không được để trống.";
            }

            if(db.Authors.SingleOrDefault(x => x.name == txtName.Text) != null)
            {
                errors += "\n - Bút danh danh này đã có trong dữ liệu rồi.";
            }

            if(errors == string.Empty)
            {
                if (edit)
                {
                    var check = db.Authors.SingleOrDefault(x => x.id.ToString() == LbId.Text);
                    if (check != null)
                    {
                        bool rs = true;
                        try
                        {
                            check.name = txtName.Text;
                            check.trueName = txtTrueName.Text;
                            check.birthday = DtpBirth.Value;
                            if (RbNam.Checked)
                            {
                                check.gender = true;
                            }
                            else
                            {
                                check.gender = false;
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
                                MessageBox.Show(this, "Tác giả " + check.name + " Đã được cập nhật.", "Thông báo");
                            }
                            else
                            {
                                MessageBox.Show(this, "Lỗi! cập nhật thất bại.", "Thông báo");
                            }
                        }
                    }
                }
                else
                {
                    Author au = new Author();
                    au.name = txtName.Text;
                    au.trueName = txtTrueName.Text;
                    au.birthday = DtpBirth.Value;
                    if (RbNam.Checked)
                    {
                        au.gender = true;
                    }
                    else
                    {
                        au.gender = false;
                    }
                    bool rs = true;
                    try
                    {
                        db.Authors.InsertOnSubmit(au);
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
                            MessageBox.Show(this, "Tác giả " + au.name + " Đã được thêm vào.", "Thông báo");
                        }
                        else
                        {
                            MessageBox.Show(this, "Lỗi! Thêm mới thất bại.", "Thông báo");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(this, errors, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
