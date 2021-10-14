using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0.frmMember
{
    public partial class DataEditForm : Form
    {
        ProjectLibraryManagementDataContext db = new ProjectLibraryManagementDataContext();
        bool edit = false;
        int id;
        public DataEditForm()
        {
            InitializeComponent();
            label8.Visible = label2.Visible = LbCreated.Visible = txtid.Visible = false;
        }

        public DataEditForm(int id)
        {
            InitializeComponent();
            edit = true;
            this.id = id;
            loadDataEdit();
        }

        public void loadDataEdit()
        {
            var info = db.Members.SingleOrDefault(x => x.id == id);
            LbCreated.Text = info.created.ToString();
            txtid.Text = info.id.ToString();
            txtFName.Text = info.firstName;
            txtLName.Text = info.lastName;
            txtPhone.Text = info.phone;
            txtEmail.Text = info.email;
            txtAddress.Text = info.address;
            DtpBirth.Value = (DateTime)info.birthday;
            if (info.gender == true)
            {
                RbNam.Checked = true;
            }
            else
            {
                RbNu.Checked = true;
            }
            if(info.image != null)
            {
                MemoryStream ms = new MemoryStream(info.image.ToArray());
                PbImage.Image = Image.FromStream(ms);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string url = string.Empty;
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    url = ofd.FileName;
                    PbImage.Image = Image.FromFile(url);
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

        private void button1_Click(object sender, EventArgs e)
        {
            string errors = "";

            if (txtFName.Text == string.Empty)
            {
                errors += "\n - Họ không được để trống.";
            }

            if (txtLName.Text == string.Empty)
            {
                errors += "\n - Tên đọc giả không được để trống.";
            }

            if (txtEmail.Text == string.Empty)
            {
                errors += "\n - Email không được để trống.";
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                errors += "\n - Định dạng Email không đúng.";
            }
            else if (db.Members.SingleOrDefault(x => x.email.Equals(txtEmail.Text)) != null)
            {
                if (edit)
                {
                    var check = db.Members.SingleOrDefault(x => x.id.ToString() == txtid.Text);
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

            if (txtPhone.Text == string.Empty)
            {
                errors += "\n - Số điện thoại không được để trống.";
            }
            else if (db.Members.SingleOrDefault(x => x.phone.Equals(txtPhone.Text)) != null)
            {
                if (edit)
                {
                    var check = db.Members.SingleOrDefault(x => x.id.ToString() == txtid.Text);
                    if (!check.phone.Equals(txtPhone.Text))
                    {
                        errors += "\n - Số điện thoại này đã được sử dụng rồi.";
                    }
                }
                else
                {
                    errors += "\n - Số điện thoại này này đã được sử dụng rồi.";
                }
            }

            if (txtAddress.Text == string.Empty)
            {
                errors += "\n - Địa chỉ không được để trống.";
            }

            if(DtpBirth.Value == null)
            {
                errors += "\n - Vui lòng xác định ngày sinh.";
            }
            else if((DateTime.Now.Year - DtpBirth.Value.Year) < 15)
            {
                errors += "\n - Để đăng ký làm đọc giả của thư viện, đọc giả tối thiểu phải 15 tuổi.";
            }

            if (edit)
            {
                if (db.Members.SingleOrDefault(x => x.phone == txtPhone.Text) != null && txtPhone.Text != db.Members.SingleOrDefault(x => x.id == id).phone)
                {
                    errors += "\n - Số điện thoại này đã được sử dụng.";
                }

                if (db.Members.SingleOrDefault(x => x.email == txtEmail.Text) != null && txtEmail.Text != db.Members.SingleOrDefault(x => x.id == id).email)
                {
                    errors += "\n - Email này đã được sử dụng.";
                }
            }
            else
            {
                if (db.Members.SingleOrDefault(x => x.phone == txtPhone.Text) != null)
                {
                    errors += "\n - Số điện thoại này đã được sử dụng.";
                }

                if (db.Members.SingleOrDefault(x => x.email == txtEmail.Text) != null)
                {
                    errors += "\n - Email này đã được sử dụng.";
                }
            }

            if (errors == string.Empty)
            {
                if (edit)
                {
                    var check = db.Members.SingleOrDefault(x => x.id.ToString() == txtid.Text);
                    if (check != null)
                    {
                        bool rs = true;
                        try
                        {
                            check.firstName = txtFName.Text;
                            check.lastName = txtLName.Text;
                            check.birthday = DtpBirth.Value;
                            check.phone = txtPhone.Text;
                            check.email = txtEmail.Text;
                            check.address = txtAddress.Text;
                            if(PbImage.Image != null)
                            {
                                Image img = PbImage.Image;
                                byte[] array;
                                ImageConverter ic = new ImageConverter();
                                array = (byte[])ic.ConvertTo(img, typeof(byte[]));
                                check.image = array;
                            }

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
                                MessageBox.Show(this, "Đọc giả " + check.lastName + " Đã được cập nhật.", "Thông báo");
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
                    Member m = new Member();
                    m.firstName = txtFName.Text;
                    m.lastName = txtLName.Text;
                    m.birthday = DtpBirth.Value;
                    m.phone = txtPhone.Text;
                    m.email = txtEmail.Text;
                    m.address = txtAddress.Text;
                    if (PbImage.Image != null)
                    {
                        Image img = PbImage.Image;
                        byte[] array;
                        ImageConverter ic = new ImageConverter();
                        array = (byte[])ic.ConvertTo(img, typeof(byte[]));
                        m.image = array;
                    }

                    if (RbNam.Checked)
                    {
                        m.gender = true;
                    }
                    else
                    {
                        m.gender = false;
                    }
                    bool rs = true;
                    try
                    {
                        db.Members.InsertOnSubmit(m);
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
                            MessageBox.Show(this, "Tác giả " + m.lastName + " Đã được thêm vào.", "Thông báo");
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
