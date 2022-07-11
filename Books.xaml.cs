using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
//using System.Windows.Forms;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for Books.xaml
    /// </summary>
    public partial class Books : Window
    {
        string username = "";
        string userfamily = "";
        int usertype = 0;
        int userid = 0;

        public Books(string username, string userfamily, int usertype, int userid)
        {
            this.username = username;
            this.userfamily = userfamily;
            this.usertype = usertype;
            this.userid = userid;

            InitializeComponent();
            if (usertype == 2)
            {
                InsertBt.IsEnabled = false;
                DeleteBt.IsEnabled = false;
                grdBooks.IsReadOnly = true;
            }
            BindDataGrid();
        }
        public void BindDataGrid()
        {
            string connetionString;
            SqlConnection cnn = null;

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            cnn.Open();

            //SELECT query to fetch data from TBLFREINDS  
            SqlDataAdapter da = new SqlDataAdapter("Select BookId,Bookname,CoverFilePath,PdfFilePath," +
                                                    "AuthorName,AuthorFamily," +
                                                    "case when AuthorType=1 then \'مولف\' when AuthorType=2 then \'مترجم\' end as AuthorType," +
                                                    "VipTag, Price, Description, PrintYear, DiscountPercent," +
                                                    "dbo.MiladyToShamsiWithSlash(DiscountExpDate),StockNumber  From Tbl_Books " +
                                                    "Where ActionType in (1,2)", cnn);
            //DataSet is virutual database or data container.  
            DataSet ds = new DataSet();
            //Fill data inside ds(DataSet) with TableNamed FriendTable  
            da.Fill(ds, "Tbl_Books");
            //Bind Data with DataGrid control.  
            grdBooks.ItemsSource = ds.Tables["Tbl_Books"].DefaultView;
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            if (usertype == 1)
            {
                AdminMenu admin = new AdminMenu(username, userfamily, usertype, userid);
                this.Close();
                admin.Show();
            }
            else
            {
                MainMenu menu = new MainMenu(username, userfamily, usertype, userid);
                this.Close();
                menu.Show();
            }
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridCell dataGridCellTarget = (DataGridCell)sender;
            string ColHeader = dataGridCellTarget.Column.Header.ToString();

            DataRowView dataRow = (DataRowView)grdBooks.SelectedItem;
            int index = grdBooks.CurrentCell.Column.DisplayIndex;
            string path = dataRow.Row.ItemArray[index].ToString();
            string bookid = dataRow.Row.ItemArray[0].ToString();
            string bookname = dataRow.Row.ItemArray[1].ToString(); ;
            string authorname = dataRow.Row.ItemArray[4].ToString()+" "+ dataRow.Row.ItemArray[5].ToString();
            int price = int.Parse(dataRow.Row.ItemArray[8].ToString());
            int stock = int.Parse(dataRow.Row.ItemArray[13].ToString());
            string sql = "SELECT AVG(Point) FROM dbo.Tbl_UserFavorites WHERE BookId=" + bookid;
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand cmd;
            int favavg = 0;

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                cmd = new SqlCommand(sql, cnn);
                if(cmd.ExecuteScalar().ToString()=="")
                {
                    favavg = 0;
                }
                else
                {
                    favavg = (int)cmd.ExecuteScalar();
                }
                cnn.Close();
                if (ColHeader == "مسير تصوير روي جلد")
                {
                    PictureShow picshow = new PictureShow(path, bookname, authorname, price, stock, favavg);
                    //this.Close();
                    picshow.Show();
                }
                if (ColHeader == "مسير فايل کتاب")
                {
                    PdfShow pdf = new PdfShow(path, usertype);
                    //this.Close();
                    pdf.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        private void InsertBt_Click(object sender, RoutedEventArgs e)
        {
            BookInsert bookins = new BookInsert(username, userfamily, usertype, userid);
            this.Close();
            bookins.Show();
        }

        private void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            string bookid = "";
            string connetionString;
            SqlConnection cnn = null;
            string sql = "";
            SqlCommand updateCommand;
            DataRowView dataRow = (DataRowView)grdBooks.SelectedItem;
            bookid = dataRow.Row.ItemArray[0].ToString();

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            sql = "UPDATE Tbl_Books set ActionType = 3 WHERE BookId = " + bookid;

            try
            {
                cnn.Open();
                updateCommand = new SqlCommand(sql, cnn);
                updateCommand.ExecuteNonQuery();
                MessageBox.Show("کتاب انتخاب شده حذف شد");
                cnn.Close();
                BindDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cnn.Close();
            }
        }

        private void FavoriteBt_Click(object sender, RoutedEventArgs e)
        {
            string bookid = "";

            DataRowView dataRow = (DataRowView)grdBooks.SelectedItem;
            bookid = dataRow.Row.ItemArray[0].ToString();
            Favorite fav = new Favorite(userid, bookid);
            fav.Show();
        }
    }
}
