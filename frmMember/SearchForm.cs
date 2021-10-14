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
    public partial class SearchForm : Form
    {
        ProjectLibraryManagementDataContext db;
        Member m = new Member();
        public SearchForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
        }

        public Member searchRs()
        {
            return m;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(txtId.Text != string.Empty)
            {
                var check = db.Members.SingleOrDefault(x => x.id.ToString() == txtId.Text);
                if(check != null)
                {
                    m = check;
                    this.Hide();
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
    }
}
