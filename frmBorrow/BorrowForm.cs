using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0.frmBorrow
{
    public partial class BorrowForm : Form
    {
        ProjectLibraryManagementDataContext db;
        List<Book> cs = new List<Book>();
        Member member = new Member();
        public BorrowForm()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            CbSearchOpt.SelectedItem = CbSearchOpt.Items[0];
        }

        public string RandomString(int length)
        {
            string str = string.Empty;
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            while (true)
            {
                str = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(6,s.Length)]).ToArray());
                if (db.Borrows.SingleOrDefault(x => x.id == str) == null)
                {
                    break;
                }
            }
            return str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id = txtId.Text;
            if(id != string.Empty)
            {
                var info = db.Members.SingleOrDefault(x => x.id.ToString() == id);
                if(info != null)
                {
                    member = info;
                    lbName.Text = info.firstName + " " + info.lastName;
                    LbPhone.Text = info.phone;
                    LbEmail.Text = info.email;
                    LbAddress.Text = info.address;
                    LbBirth.Text = info.birthday.Value.ToString("dd/MM/yyyy");
                    if (info.gender == true)
                    {
                        LbGender.Text = "Nam";
                    }
                    else
                    {
                        LbGender.Text = "Nữ";
                    }
                    if (info.image != null)
                    {
                        MemoryStream ms = new MemoryStream(info.image.ToArray());
                        PbImage.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đọc giả này.", "Thông báo");
                }
            }
            else
            {
                MessageBox.Show("Hãy nhập vào mã đọc giả.", "Nhắc nhở");
            }
        }

        public void bookSearch()
        {
            string kw = txtKeyWord.Text;
            if (kw != string.Empty)
            {
                var books = db.Books.ToList();
                string opt = CbSearchOpt.SelectedItem.ToString();
                if (opt.Equals("Tên đầu sách"))
                {
                    books = db.Books.Where(x => x.name.Contains(kw)).ToList();
                }
                else if (opt.Equals("Mã đầu sách"))
                {
                    books = db.Books.Where(x => x.id.ToString().Contains(kw)).ToList();
                }
                else
                {
                    books = db.AuthorsBooks.Where(ab => ab.Author.name.Contains(kw)).Select(ab => ab.Book).ToList();
                }

                DgvBook.Rows.Clear();
                foreach (var b in books)
                {
                    DgvBook.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, (b.status == true) ? "còn" : "hết");
                }
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            bookSearch();
        }

        private void DgvBook_Click(object sender, EventArgs e)
        {
            if(DgvBook.Rows.Count > 0)
            {
                var row = DgvBook.CurrentRow;
                int id = int.Parse(row.Cells[0].Value.ToString());
                var book = db.Books.SingleOrDefault(x => x.id == id);
                if(book.status == true)
                {
                    if (!cs.Contains(book))
                    {
                        cs.Add(book);
                        DgvBookChoose.Rows.Add(book.id, book.name);
                    }
                }
                else
                {
                    MessageBox.Show("Cuốn sách hiện đạng được mượn hoặc không còn.", "Thông báo");
                }
            }
        }

        private void DgvBookChoose_DoubleClick(object sender, EventArgs e)
        {
            if (DgvBookChoose.Rows.Count > 0)
            {
                var row = DgvBookChoose.CurrentRow;
                int id = int.Parse(row.Cells[0].Value.ToString());
                var book = db.Books.SingleOrDefault(x => x.id == id);
                cs.Remove(book);
                DgvBookChoose.Rows.Remove(row);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DgvBook.Rows.Clear();
            DgvBookChoose.Rows.Clear();
            txtId.Text = lbName.Text = LbPhone.Text = LbEmail.Text = LbAddress.Text = LbBirth.Text = LbGender.Text = txtKeyWord.Text = string.Empty;

            member = null;
            cs.Clear();

            if (PbImage.Image != null)
            {
                PbImage.Image = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string errors = string.Empty;

            if(cs.Count == 0)
            {
                errors += "\n - Bạn chưa chọn cuốn đầu sách nào, Vui lòng chọn tối thiểu 1 đầu sách.";
            }

            if(member == null)
            {
                errors += "\n - Vui lòng xác định đọc giả cần mượn sách.";
            }

            if(errors.Length == 0)
            {
                Borrow br = new Borrow();
                bool rs = true;
                try {
                    br.id = RandomString(new Random().Next(10));
                    br.memberId = member.id;
                    br.totalBook = cs.Count;
                    br.status = false;
                    br.totalFee = 0;
                    db.Borrows.InsertOnSubmit(br);
                    List<BorrowDetail> bd = new List<BorrowDetail>();
                    foreach(Book b in cs)
                    {
                        b.status = false;
                        BorrowDetail info = new BorrowDetail();
                        info.borrowId = br.id;
                        info.bookId = b.id;
                        bd.Add(info);
                    }
                    db.BorrowDetails.InsertAllOnSubmit(bd);
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
                        MessageBox.Show(this, "Phiếu mượn " + br.id + " Đã được tạo.", "Thông báo");
                        bookSearch();
                    }
                    else
                    {
                        MessageBox.Show(this, "Lỗi! Phiếu mượn tạo không thành công.", "Thông báo");
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
