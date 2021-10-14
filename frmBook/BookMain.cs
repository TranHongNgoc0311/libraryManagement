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
    public partial class BookMain : Form
    {
        ProjectLibraryManagementDataContext db;
        List<AuthorsBook> newAuthorList = new List<AuthorsBook>();
        bool edit = false;
        public BookMain()
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();
            showList();
            loadCat();

            CbFilterCat.DataSource = db.Categories.Where(x => x.status == true).ToList();
            CbFilterCat.DisplayMember = "name";
            CbFilterCat.ValueMember = "id";

            CbAuthor.DataSource = db.Authors.ToList();
            CbAuthor.DisplayMember = "name";
            CbAuthor.ValueMember = "id";
            CbSearchOpt.SelectedItem = CbSearchOpt.Items[0];
        }

        public BookMain(List<Book> searchRs)
        {
            InitializeComponent();
            db = new ProjectLibraryManagementDataContext();

            loadCat();

            CbFilterCat.DataSource = db.Categories.Where(x => x.status == true).ToList();
            CbFilterCat.DisplayMember = "name";
            CbFilterCat.ValueMember = "id";

            CbAuthor.DataSource = db.Authors.ToList();
            CbAuthor.DisplayMember = "name";
            CbAuthor.ValueMember = "id";
            CbSearchOpt.SelectedItem = CbSearchOpt.Items[0];

            DgvBooks.Rows.Clear();
            foreach (var b in searchRs)
            {
                DgvBooks.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, b.pages, b.price, b.shelf_no, (b.status == true) ? "còn" : "hết");
            }
        }

        public void loadCat()
        {
            if (edit)
            {
                Cbcat.DataSource = db.Categories.ToList();
            }
            else
            {
                Cbcat.DataSource = db.Categories.Where(x => x.status == true).ToList();
            }
            Cbcat.DisplayMember = "name";
            Cbcat.ValueMember = "id";
        }

        public void showList()
        {
            DgvBooks.Rows.Clear();
            var books = db.Books.ToList();
            foreach(var b in books)
            {
                DgvBooks.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, b.pages, b.price, b.shelf_no, (b.status == true) ? "còn" : "hết");
            }
        }

        private void DgvBooks_Click(object sender, EventArgs e)
        {
            if (DgvBooks.Rows.Count > 0)
            {
                edit = true;
                loadCat();
                getRecordData();
                newAuthorList = db.AuthorsBooks.Where(ab => ab.bookId == int.Parse(txtId.Text)).ToList();
            }
        }

        public void displayAuthor(List<AuthorsBook> list)
        {
            DgvAuthorI4.Rows.Clear();
            foreach(var i4 in list)
            {
                DgvAuthorI4.Rows.Add(i4.Author.name, i4.position);
            }
        }

        public void getRecordData()
        {
            if(DgvBooks.Rows.Count > 0)
            {
                DataGridViewRow view = DgvBooks.CurrentRow;
                int id = int.Parse(view.Cells[0].Value.ToString());
                var target = db.Books.Where(b => b.id == id).SingleOrDefault();
                displayAuthor(db.AuthorsBooks.Where(ab => ab.bookId == id).ToList());
                txtId.Text = target.id.ToString();
                txtName.Text = target.name;
                LbCreate.Text = target.created.ToString();
                txtNXB.Text = target.publisher;
                txtPage.Value = decimal.Parse(target.pages.ToString());
                Cbcat.SelectedValue = target.Category.id;
                txtShelf.Value = decimal.Parse(target.shelf_no.ToString());
                txtYear.Text = target.bookYear.ToString();
                txtPrice.Value = decimal.Parse(target.price.ToString());
                if (target.status == true)
                {
                    RbCon.Checked = true;
                }
                else
                {
                    RbHet.Checked = true;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string errors = "";
            if(txtPst.Text == string.Empty)
            {
                errors += "\n Vui lòng điền vào đóng góp của tác giả với sản phẩm";
            }
            if (errors == string.Empty)
            {
                int aId = int.Parse(CbAuthor.SelectedValue.ToString());
                if (edit)
                {
                    int bID = int.Parse(txtId.Text);
                    var check = db.AuthorsBooks.SingleOrDefault(x => x.bookId == bID && x.AuthorId == aId && x.position == txtPst.Text);
                    if (check != null)
                    {
                        MessageBox.Show(this, "Đã thêm tác giả với vị trí này rồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        AuthorsBook ab = new AuthorsBook();
                        ab.AuthorId = aId;
                        ab.position = txtPst.Text;
                        newAuthorList.Add(ab);
                        DgvAuthorI4.Rows.Add(db.Authors.SingleOrDefault(x => x.id == ab.AuthorId).name, ab.position);
                    }
                }
                else
                {
                    AuthorsBook ab = new AuthorsBook();
                    ab.AuthorId = aId;
                    ab.position = txtPst.Text;
                    newAuthorList.Add(ab);
                    DgvAuthorI4.Rows.Add(db.Authors.SingleOrDefault(x => x.id == ab.AuthorId).name, ab.position);
                }
            }
            else
            {
                MessageBox.Show(this, errors, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvAuthorI4_Click(object sender, EventArgs e)
        {
            if(DgvAuthorI4.Rows.Count > 0)
            {
                DataGridViewRow row = DgvAuthorI4.CurrentRow;
                CbAuthor.SelectedValue = db.Authors.Where(a => a.name == row.Cells[0].Value.ToString()).SingleOrDefault().id;
                txtPst.Text = row.Cells[1].Value.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            edit = false;
            loadCat();
            foreach (var c in this.groupBox1.Controls)
            {
                var text = c as TextBox;
                if(text != null)
                {
                    text.Text = "";
                }
            }
            txtPrice.Value = 0;
            txtPage.Value = 0;
            txtShelf.Value = 0;
            txtId.Focus();
            DgvAuthorI4.Rows.Clear();
            newAuthorList.Clear();
            txtId.ReadOnly = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            getRecordData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string errors = "";

            if(txtName.Text.Length == 0)
            {
                errors += "\n- Tên đầu sách không được để trống";
            }
            else if(db.Books.SingleOrDefault(x => x.name.Equals(txtName.Text)) != null)
            {
                if (edit)
                {
                    var check = db.Books.SingleOrDefault(x => x.id == int.Parse(txtId.Text));
                    if (!check.name.Equals(txtName.Text))
                    {
                        errors += "\n- Tên đầu sách này đẫ được sử dụng rồi";
                    }
                }
                else
                {
                    errors += "\n- Tên đầu sách này đẫ được sử dụng rồi";
                }
            }

            if(txtYear.Text != string.Empty)
            {
                if (int.Parse(txtYear.Text) > DateTime.Now.Year)
                {
                    errors += "\n - Năm xuất bản không thể nào lớn hơn năm hiện tại.";
                }
            }

            if(txtNXB.Text.Length == 0)
            {
                errors += "\n- Nhà xuất bản không được để trống";
            }
            if(txtPage.Value == 0 || txtPage.Value.ToString() == String.Empty)
            {
                errors += "\n- Số trang sách không được để trống hoặc bằng 0";
            }
            if(txtPrice.Value.ToString() == String.Empty)
            {
                errors += "\n- Giá đầu sách không được để trống";
            }
            if(txtShelf.Value == 0 || txtShelf.Value.ToString() == String.Empty)
            {
                errors += "\n- Số giá sách không được để trống hoặc bằng 0";
            }

            if(errors == string.Empty)
            {
                Book book = new Book();
                book.name = txtName.Text;
                book.publisher = txtNXB.Text;
                book.pages = Convert.ToInt32(txtPage.Value);
                book.price = Convert.ToDouble(txtPrice.Value);
                book.catID = int.Parse(Cbcat.SelectedValue.ToString());
                book.shelf_no = Convert.ToInt32(txtShelf.Value);
                book.bookYear = int.Parse(txtYear.Text);
                if (RbCon.Checked)
                {
                    book.status = true;
                }
                else
                {
                    book.status = false;
                }

                if (!edit)
                {
                    bool rs = true;
                    try
                    {
                        db.Books.InsertOnSubmit(book);
                        db.SubmitChanges();
                        foreach (var ab in newAuthorList)
                        {
                            ab.bookId = db.Books.Where(b => b.name == book.name).SingleOrDefault().id;
                        }
                        db.AuthorsBooks.InsertAllOnSubmit(newAuthorList);
                        displayAuthor(db.AuthorsBooks.Where(x => x.bookId == book.id).ToList());
                        db.SubmitChanges();
                    }
                    catch(Exception )
                    {
                        rs = false;
                    }
                    finally
                    {
                        if (rs)
                        {
                            MessageBox.Show(this, "Cuốn sách " + book.name + " Đã được thêm vào.", "Thông báo");
                        }
                        else
                        {
                            MessageBox.Show(this, "Lỗi! cuốn sách không thể thêm.", "Thông báo");
                        }
                    }
                }
                else
                {
                    var check = db.Books.SingleOrDefault(x => x.id == int.Parse(txtId.Text));
                    if (check != null)
                    {
                        bool rs = true;
                        try
                        {
                            check.name = txtName.Text;
                            check.publisher = txtNXB.Text;
                            check.bookYear = int.Parse(txtYear.Text);
                            check.pages = Convert.ToInt32(txtPage.Value);
                            check.price = Convert.ToDouble(txtPrice.Value);
                            check.catID = int.Parse(Cbcat.SelectedValue.ToString());
                            check.shelf_no = Convert.ToInt32(txtShelf.Value);
                            db.AuthorsBooks.DeleteAllOnSubmit(db.AuthorsBooks.Where(x => x.bookId == check.id));
                            if (RbCon.Checked)
                            {
                                check.status = true;
                            }
                            else
                            {
                                check.status = false;
                            }
                            db.SubmitChanges();
                            foreach (var ab in newAuthorList)
                            {
                                ab.bookId = check.id;
                            }
                            db.AuthorsBooks.InsertAllOnSubmit(newAuthorList);
                            displayAuthor(db.AuthorsBooks.Where(x => x.bookId == check.id).ToList());
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
                                MessageBox.Show(this, "Cuốn sách " + check.name + " Đã được Cập nhật.", "Thông báo");
                            }
                            else
                            {
                                MessageBox.Show(this, "Lỗi! Cập nhật thất bại.", "Thông báo");
                            }
                        }
                    }
                }

                showList();
            }
            else
            {
                MessageBox.Show(this, errors, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show(this, "Bạn chắc chắn muốn xóa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.Yes)
            {
                int aId = int.Parse(CbAuthor.SelectedValue.ToString());
                int bID = int.Parse(txtId.Text);
                AuthorsBook ab = new AuthorsBook();
                ab.AuthorId = aId;
                ab.position = txtPst.Text;
                newAuthorList.Remove(ab);
                DgvAuthorI4.Rows.Add(db.Authors.Select(a => a.name), ab.position);
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show(this, "Bạn chắc chắn muốn xóa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.Yes)
            {
                bool check = true;
                try
                {
                    int bId = int.Parse(txtId.Text);
                    db.AuthorsBooks.DeleteAllOnSubmit(db.AuthorsBooks.Where(x => x.bookId == bId).ToList());
                    db.BorrowDetails.DeleteAllOnSubmit(db.BorrowDetails.Where(x => x.bookId == bId).ToList());
                    db.Books.DeleteOnSubmit(db.Books.SingleOrDefault(x => x.id == bId));
                    db.SubmitChanges();
                    showList();
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

        private void txtKeyWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            

        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtKeyWord_TextChanged(object sender, EventArgs e)
        {
            var books = db.Books.ToList();
            string kw = txtKeyWord.Text;
            string opt = CbSearchOpt.SelectedItem.ToString();
            if (opt.Equals("Tên đầu sách"))
            {
                books = db.Books.Where(x => x.name.Contains(kw)).ToList();
            }
            else if (opt.Equals("Mã đầu sách"))
            {
                books = db.Books.Where(x => x.id.ToString() == kw).ToList();
            }
            else
            {
                books = db.AuthorsBooks.Where(ab => ab.Author.name.Contains(kw)).Select(ab => ab.Book).ToList();
            }

            DgvBooks.Rows.Clear();
            foreach (var b in books)
            {
                DgvBooks.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, b.pages, b.price, b.shelf_no, (b.status == true) ? "còn" : "hết");
            }
        }

        private void CbFilterCat_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            showList();
            showAll = true;
        }

        private void CbFilterCat_DropDownClosed(object sender, EventArgs e)
        {
            
        }

        bool showAll = true;
        private void CbFilterCat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            List<Book> books = new List<Book>();
            int catID = int.Parse(CbFilterCat.SelectedValue.ToString());
            if (RbFilterStatusCon.Checked)
            {
                books = db.Books.Where(x => x.catID == catID && x.status == true).ToList();
            }else if (RbFilterStatusHet.Checked)
            {
                books = db.Books.Where(x => x.catID == catID && x.status == false).ToList();
            }
            else
            {
                books = db.Books.Where(x => x.catID == catID).ToList();
            }

            DgvBooks.Rows.Clear();
            foreach (var b in books)
            {
                DgvBooks.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, b.pages, b.price, b.shelf_no, (b.status == true) ? "còn" : "hết");
            }
            showAll = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (showAll)
            {
                showList();
            }
            else
            {
                int catID = int.Parse(CbFilterCat.SelectedValue.ToString());
                var books = db.Books.Where(x => x.catID == catID).ToList();
                DgvBooks.Rows.Clear();
                foreach (var b in books)
                {
                    DgvBooks.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, b.pages, b.price, b.shelf_no, (b.status == true) ? "còn" : "hết");
                }
            }
        }

        private void RbFilterStatusHet_CheckedChanged(object sender, EventArgs e)
        {
            List<Book> books = new List<Book>();
            if (showAll)
            {
                books = db.Books.Where(x => x.status == false).ToList();
            }
            else
            {
                int catID = int.Parse(CbFilterCat.SelectedValue.ToString());
                books = db.Books.Where(x => x.catID == catID && x.status == false).ToList();
            }
            DgvBooks.Rows.Clear();
            foreach (var b in books)
            {
                DgvBooks.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, b.pages, b.price, b.shelf_no, (b.status == true) ? "còn" : "hết");
            }
        }

        private void RbFilterStatusCon_CheckedChanged(object sender, EventArgs e)
        {
            List<Book> books = new List<Book>();
            if (showAll)
            {
                books = db.Books.Where(x => x.status == true).ToList();
            }
            else
            {
                int catID = int.Parse(CbFilterCat.SelectedValue.ToString());
                books = db.Books.Where(x => x.catID == catID && x.status == true).ToList();
            }
            DgvBooks.Rows.Clear();
            foreach (var b in books)
            {
                DgvBooks.Rows.Add(b.id, b.name, b.Category.name, b.publisher, b.bookYear, b.pages, b.price, b.shelf_no, (b.status == true) ? "còn" : "hết");
            }
        }
    }
}
