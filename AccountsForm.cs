using LibraMaster_ver2._0.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0
{
    public partial class AccountsForm : Form
    {
        ProjectLibraryManagementDataContext db;
        bool edit = false;
        public AccountsForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();

            DgvAcc.DataSource = db.Accounts.Select(x => new { x.id,x.name,x.email,x.password,level = (x.level == 0)?"Quản lý":"Thủ thư",x.created });
        }

        public void getRecordData()
        {
            if (DgvAcc.Rows.Count > 0)
            {
                DataGridViewRow view = DgvAcc.CurrentRow;
                int id = int.Parse(view.Cells[0].Value.ToString());
                var target = db.Accounts.SingleOrDefault(a => a.id == id);
                txtEmail.Text = target.email;
                txtId.Text = target.id.ToString();
                txtName.Text = target.name;
                txtPwd.Text = target.password;
            }
        }

        private void DgvAcc_Click(object sender, EventArgs e)
        {
            if (DgvAcc.Rows.Count > 0)
            {
                getRecordData();
                edit = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            edit = false;
            foreach (var c in this.panel2.Controls)
            {
                var text = c as TextBox;
                if (text != null)
                {
                    text.Text = "";
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            getRecordData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show(this, "Bạn chắc chắn muốn xóa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.Yes)
            {
                int id = int.Parse(txtId.Text);
                if(UserLoginData.Userdata.level > db.Accounts.SingleOrDefault(x => x.id == id).level)
                {
                    bool check = true;
                    try
                    {
                        db.Accounts.DeleteOnSubmit(db.Accounts.SingleOrDefault(x => x.id == id));
                        db.SubmitChanges();
                        DgvAcc.DataSource = db.Accounts.Select(x => new { x.id, x.name, x.email, x.password, level = (x.level == 0) ? "Quản lý" : "Thủ thư", x.created });
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
                else
                {
                    MessageBox.Show(this, "Bạn không thể thực hiện thao tác này.", "Thông báo");
                }
            }
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string errors = "";

            if (txtName.Text.Length == 0)
            {
                errors += "\n- Họ và tên không được để trống";
            }
            if (txtEmail.Text.Length == 0)
            {
                errors += "\n- Email không được để trống";
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                errors += "\n - Định dạng Email không đúng.";
            }
            else if (db.Accounts.SingleOrDefault(x => x.email.Equals(txtEmail.Text)) != null)
            {
                if (edit)
                {
                    var check = db.Accounts.SingleOrDefault(x => x.id.ToString() == txtId.Text);
                    if (!check.email.Equals(txtEmail.Text))
                    {
                        errors += "\n - Email này đã được sử dụng rồi.";
                    }
                }
                else
                {
                    errors += "\n - Email này này đã được sử dụng rồi.";
                }
            }

            if (txtPwd.ToString() == String.Empty)
            {
                errors += "\n- Mật khẩu không được để trống";
            }
            else if (txtPwd.Text.Length < 6)
            {
                errors += "\n- Mật khẩu tối thiểu 6 ký tự.";
            }

            if (errors == string.Empty)
            {
                Account acc = new Account();
                acc.name = txtName.Text;
                acc.email = txtEmail.Text;
                acc.password = txtPwd.Text;
                acc.level = 1;

                if (!edit)
                {
                    bool rs = true;
                    try
                    {
                        db.Accounts.InsertOnSubmit(acc);
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
                            MessageBox.Show(this, "Tài khoản của " + acc.name + " Đã được thêm vào.", "Thông báo");
                        }
                        else
                        {
                            MessageBox.Show(this, "Lỗi! Thêm mới thất bại.", "Thông báo");
                        }
                    }
                }
                else
                {
                    var check = db.Accounts.SingleOrDefault(x => x.id == int.Parse(txtId.Text));
                    if(UserLoginData.Userdata.level >= check.level)
                    {
                        if (check != null)
                        {
                            bool rs = true;
                            try
                            {
                                check.name = txtName.Text;
                                check.email = txtEmail.Text;
                                check.password = txtPwd.Text;
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
                                    MessageBox.Show(this, "Tài khoản của " + check.name + " Đã được Cập nhật.", "Thông báo");
                                }
                                else
                                {
                                    MessageBox.Show(this, "Lỗi! Cập nhật thất bại.", "Thông báo");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Bạn không thể thực hiện thao tác này.", "Thông báo");
                    }
                }

                DgvAcc.DataSource = db.Accounts.Select(x => new { x.id, x.name, x.email, x.password, level = (x.level == 0) ? "Quản lý" : "Thủ thư", x.created });
            }
            else
            {
                MessageBox.Show(this, errors, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool passwordView = false;
        private void button5_Click(object sender, EventArgs e)
        {
            if (passwordView)
            {
                txtPwd.UseSystemPasswordChar = false;
                passwordView = false;
            }
            else
            {
                txtPwd.UseSystemPasswordChar = true;
                passwordView = true;
            }
        }
    }
}
