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
    public partial class PasswordChangeForm : Form
    {
        ProjectLibraryManagementDataContext db;
        public PasswordChangeForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string errors = "";

            if(txtNew.Text == string.Empty)
            {
                errors += "\n - Vui lòng nhập vào mật khẩu mới.";
            }
            else if(txtNew.Text.Length < 6)
            {
                errors += "\n - Mật khẩu tối thiểu 6 ký tự.";
            }

            if (!txtConfirm.Text.Equals(txtNew.Text))
            {
                errors += "\n - Mật khẩu không khớp.";
            }

            if(txtPw.Text == string.Empty)
            {
                errors += "\n - Vui lòng nhập vào mật khẩu hiện tại.";
            }
            else if (!UserLoginData.Userdata.password.Equals(txtPw.Text))
            {
                errors += "\n - Mật khẩu không chính xác.";
            }

            if(errors == string.Empty)
            {
                bool rs = true;
                try
                {
                    var a = db.Accounts.SingleOrDefault(x => x.id == UserLoginData.Userdata.id);
                    a.password = txtNew.Text;
                    Properties.Settings.Default.Password = txtNew.Text;
                    Properties.Settings.Default.Save();
                    db.SubmitChanges();
                    UserLoginData.Userdata = a;
                }
                catch (Exception)
                {
                    rs = false;
                }
                finally
                {
                    if (rs)
                    {
                        MessageBox.Show(this, "Đổi mật khẩu thành công.", "Thông báo");
                    }
                    else
                    {
                        MessageBox.Show(this, "Lỗi! đổi mật khẩu thất bại.", "Thông báo");
                    }
                }
            }
            else
            {
                MessageBox.Show(this, errors, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool c1 = true;
        private void button1_Click(object sender, EventArgs e)
        {
            if(c1 == true)
            {
                txtNew.UseSystemPasswordChar = false;
                c1 = false;
            }
            else
            {
                txtNew.UseSystemPasswordChar = true;
                c1 = true;
            }
        }

        bool c2 = true;
        private void button4_Click(object sender, EventArgs e)
        {
            if (c2 == true)
            {
                txtConfirm.UseSystemPasswordChar = false;
                c2 = false;
            }
            else
            {
                txtConfirm.UseSystemPasswordChar = true;
                c2 = true;
            }
        }

        bool c3 = true;
        private void button3_Click(object sender, EventArgs e)
        {
            if (c3 == true)
            {
                txtPw.UseSystemPasswordChar = false;
                c3 = false;
            }
            else
            {
                txtPw.UseSystemPasswordChar = true;
                c3 = true;
            }
        }
    }
}
