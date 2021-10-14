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
    public partial class HomeForm : Form
    {
        ProjectLibraryManagementDataContext db;
        public HomeForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            txtId.Text = UserLoginData.Userdata.id.ToString();
            txtCreated.Text = UserLoginData.Userdata.created.Value.ToString("dd/MM/yyyy");
            txtEmail.Text = UserLoginData.Userdata.email;
            txtLevel.Text = (UserLoginData.Userdata.level == 0) ? "Quản lý" : "Thủ thư";
            txtName.Text = UserLoginData.Userdata.name;
            LbBookCount.Text = db.Books.Count().ToString();
            LbBrCount.Text = db.Borrows.Where(x => x.status == true).Count().ToString();
            LbLiCount.Text = db.Accounts.Count().ToString();
            LbMemCount.Text = db.Members.Count().ToString();
            txtNote.Text = Properties.Settings.Default.Note;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (txtKeyWord.Text != string.Empty)
            {
                string keyWord = txtKeyWord.Text;
                var rs = db.Books.Where(b => b.id.ToString() == keyWord ||
                b.bookYear.ToString() == keyWord ||
                b.name.Contains(keyWord) || b.publisher.Contains(keyWord)).ToList();

                if (rs != null)
                {
                    var parent = this.MdiParent;
                    parent.ActiveMdiChild.Close();
                    frmBook.BookMain form = new frmBook.BookMain(rs);
                    form.MdiParent = parent;
                    form.Dock = DockStyle.Fill;
                    form.ControlBox = false;
                    form.Show();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập vào từ khóa.", "Nhắc nhở");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (txtMemberId.Text != string.Empty)
            {
                var check = db.Members.SingleOrDefault(x => x.id.ToString() == txtMemberId.Text);
                if (check != null)
                {
                    frmMember.DetailForm form = new frmMember.DetailForm(check.id);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đọc giả này.", "Thông báo");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập vào mã đọc giả.", "Nhắc nhở");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtEmail.Text = UserLoginData.Userdata.email;
            txtName.Text = UserLoginData.Userdata.name;
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

        private void button1_Click(object sender, EventArgs e)
        {
            string errors = "";

            if(txtEmail.Text.Length == 0)
            {
                errors += "\n - Vui lòng nhập vào email.";
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                errors += "\n - Định dạng Email không đúng.";
            }

            if(db.Accounts.SingleOrDefault(x =>x.email.Equals(txtEmail.Text)) != null && !UserLoginData.Userdata.email.Equals(txtEmail.Text))
            {
                errors += "\n - Email đã được sử dụng.";
            }

            if(txtName.Text.Length == 0)
            {
                errors += "\n - Vui lòng nhập vào họ và tên.";
            }

            if(errors.Length == 0)
            {
                bool rs = true;
                try
                {
                    var a = db.Accounts.SingleOrDefault(x => x.id.ToString() == txtId.Text);
                    a.email = txtEmail.Text;
                    a.name = txtName.Text;
                    UserLoginData.Userdata.email = txtEmail.Text;
                    Properties.Settings.Default.Email = txtEmail.Text;
                    Properties.Settings.Default.Save();
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
                        MessageBox.Show(this, "Cập nhật thành công.", "Thông báo");
                        txtEmail.Text = UserLoginData.Userdata.email;
                        txtName.Text = UserLoginData.Userdata.name;
                    }
                    else
                    {
                        MessageBox.Show(this, "Lỗi! cập nhật thất bại.", "Thông báo");
                    }
                }
            }
            else
            {
                MessageBox.Show(this, errors, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Note = txtNote.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show(this, "Đã lưu ghi chú của bạn.", "Thông báo");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PasswordChangeForm form = new PasswordChangeForm();
            form.ShowDialog();
        }
    }
}
