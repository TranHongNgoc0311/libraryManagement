using LibraMaster_ver2._0.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0
{
    public partial class LoginForm : Form
    {
        ProjectLibraryManagementDataContext db;
        List<Account> accounts = new List<Account>();
        public LoginForm()
        {
            InitializeComponent();

            db = new ProjectLibraryManagementDataContext();
            accounts = db.Accounts.ToList();

            panelBanner.BackColor = Color.FromArgb(150, Color.White);
            panel3.BackColor = Color.FromArgb(0, Color.White);
            panelForm.BackColor = Color.FromArgb(150, Color.Black);
        }

        public void rememberMe()
        {
            if (CbRememberMe.Checked == true)
            {
                Properties.Settings.Default.Email = txtEmail.Text;
                Properties.Settings.Default.Password = txtPassword.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void panel2_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            PlButtonClose.BackColor = Color.FromArgb(63, 63, 63);
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            PlButtonClose.BackColor = Color.Transparent;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PlButtonClose_MouseEnter(object sender, EventArgs e)
        {
            PlButtonClose.BackColor = Color.FromArgb(63, 63, 63);
        }

        private void PlButtonClose_MouseLeave(object sender, EventArgs e)
        {
            PlButtonClose.BackColor = Color.Transparent;
        }

        private void PlButtonClose_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string pwd = txtPassword.Text;

            Account acc = db.Accounts.SingleOrDefault(x=>x.email == email);
            if(acc != null)
            {
                if (acc.password.Equals(pwd))
                {
                    rememberMe();
                    UserLoginData.Userdata = acc;
                    this.Hide();
                    Waiting form = new Waiting();
                    form.Show();
                }
                else
                {
                    MessageBox.Show(this, "Mật khẩu không chính xác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(this,"Email không chính xác hoặc không tồn tại.","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void CbRememberMe_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}
