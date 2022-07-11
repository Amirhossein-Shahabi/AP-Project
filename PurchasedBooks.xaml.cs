using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for PurchasedBooks.xaml
    /// </summary>

    public partial class PurchasedBooks : Window
    {
        string username = "";
        string userfamily = "";
        int usertype = 0;
        int userid = 0;
        public PurchasedBooks(string username, string userfamily, int usertype, int userid)
        {
            this.username = username;
            this.userfamily = userfamily;
            this.usertype = usertype;
            this.userid = userid;

            InitializeComponent();
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
            SqlDataAdapter da = new SqlDataAdapter("Select T2.BookId,T2.Bookname,T2.CoverFilePath,T2.PdfFilePath," +
                                                    "T2.AuthorName,T2.AuthorFamily," +
                                                    "case when T2.AuthorType=1 then \'مولف\' when T2.AuthorType=2 then \'مترجم\' end as AuthorType," +
                                                    "T2.VipTag, T2.Price, T2.Description, T2.PrintYear, T2.DiscountPercent," +
                                                    "dbo.MiladyToShamsiWithSlash(T2.DiscountExpDate),T2.StockNumber  From Tbl_Purchase AS T1 LEFT JOIN Tbl_Books AS T2 ON T1.BookId=T2.BookId " +
                                                    "Where T1.ActionType in (1,2) AND T1.UserId='"+userid.ToString()+@"'", cnn);
            //DataSet is virutual database or data container.  
            DataSet ds = new DataSet();
            //Fill data inside ds(DataSet) with TableNamed FriendTable  
            da.Fill(ds, "Tbl_Purchase");
            //Bind Data with DataGrid control.  
            grdPurchased.ItemsSource = ds.Tables["Tbl_Purchase"].DefaultView;
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            MainMenu menu = new MainMenu(username, userfamily, usertype, userid);
            this.Close();
            menu.Show();
        }
        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridCell dataGridCellTarget = (DataGridCell)sender;
            string ColHeader = dataGridCellTarget.Column.Header.ToString();

            DataRowView dataRow = (DataRowView)grdPurchased.SelectedItem;
            int index = grdPurchased.CurrentCell.Column.DisplayIndex;
            string path = dataRow.Row.ItemArray[index].ToString();
            string bookid = dataRow.Row.ItemArray[0].ToString();
            string bookname = dataRow.Row.ItemArray[1].ToString(); ;
            string authorname = dataRow.Row.ItemArray[4].ToString() + " " + dataRow.Row.ItemArray[5].ToString();
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
                if (cmd.ExecuteScalar().ToString() == "")
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
    }
}
