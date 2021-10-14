using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data = System.Collections.Generic.KeyValuePair<int, string>;

namespace LibraMaster_ver2._0.frmBook
{
    public partial class SearchBook : Form
    {
        ProjectLibraryManagementDataContext db;
        List<Book> rs;
        public SearchBook()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
        }
        public List<Book> searchRs()
        {
            if(rs != null)
            {
                MessageBox.Show(this, "Kết quả tìm kiếm: " + rs.Count() + " Kết quả", "Tìm kiếm");
            }
            return rs;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string keyWord = txtKeyWord.Text;
            /*var rs = from b in db.Books
                     where SqlMethods.Like(b.name, "%" + keyWord + "%") || 
                     SqlMethods.Like(b.publisher, "%" + keyWord + "%") || 
                     b.id == int.Parse(keyWord) || b.bookYear == int.Parse(keyWord) 
                     select b;*/
            rs = db.Books.Where(b => b.id.ToString() == keyWord || 
            b.bookYear.ToString() == keyWord || 
            b.name.Contains(keyWord) || b.publisher.Contains(keyWord)).ToList();
            this.Hide();
        }
    }
}
