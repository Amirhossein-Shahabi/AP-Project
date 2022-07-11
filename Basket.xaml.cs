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
using System.Data.SqlClient;
using System.Data;


namespace WpfProject
{
    /// <summary>
    /// Interaction logic for Basket.xaml
    /// </summary>
    public partial class Basket : Window
    {
        string username = "";
        string userfamily = "";
        int usertype = 0;
        int userid = 0;
        int totalamount = 0;

        public Basket(string username, string userfamily, int usertype, int userid)
        {
            this.username = username;
            this.userfamily = userfamily;
            this.usertype = usertype;
            this.userid = userid;

            InitializeComponent();
//            PurchaseValletBt.IsEnabled = false;
//            PurchaseInternetBt.IsEnabled = false;
//            DeleteBt.IsEnabled = false;
//            grdBasket.IsEnabled = false;
            BindDataGrid();
            BindBasketGrid();
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
            MainMenu menu = new MainMenu(username, userfamily, usertype, userid);
            this.Close();
            menu.Show();
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

        private void FavoriteBt_Click(object sender, RoutedEventArgs e)
        {
            string bookid = "";

            DataRowView dataRow = (DataRowView)grdBooks.SelectedItem;
            bookid = dataRow.Row.ItemArray[0].ToString();
            Favorite fav = new Favorite(userid, bookid);
            fav.Show();
        }

        private void InsertBt_Click(object sender, RoutedEventArgs e)
        {
            PurchaseValletBt.IsEnabled = true;
            PurchaseInternetBt.IsEnabled = true;
            DeleteBt.IsEnabled = true;
            grdBasket.IsEnabled = true;

            string connetionString;
            SqlConnection cnn = null;
            SqlCommand sql_cmnd, command;
            int amount = 0;
            string discount = "";
            string date = "";
            string price = "";
            string sql = "";
            string stock = "";
            string bookid = "";
            DateTime now = DateTime.Now;
            DateTime ndate;

            sql = "select ";


            DataRowView dataRow = (DataRowView)grdBooks.SelectedItem;
//            int index = grdBooks.CurrentCell.Column.DisplayIndex;
            bookid = dataRow.Row.ItemArray[0].ToString();
            date = dataRow.Row.ItemArray[12].ToString();
            string[] dat = date.Split('/');
            date = dat[0] + dat[1] + dat[2];
            discount = dataRow.Row.ItemArray[11].ToString();
            price = dataRow.Row.ItemArray[8].ToString();
            stock = dataRow.Row.ItemArray[13].ToString();

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            sql = "SELECT dbo.ShamsiToMilady(" + date + ")";
            command = new SqlCommand(sql, cnn);
            command.CommandType = CommandType.Text;

            try
            {
                if (int.Parse(stock) == 0)
                {
                    throw new FormatException("موجودي به اتمام رسيده است");
                }
                cnn.Open();
                if(date != "")
                {
                    ndate = (DateTime)command.ExecuteScalar();
                    if (ndate <= now)
                    {
                        amount = int.Parse(price) - int.Parse(price) * int.Parse(discount);
                    }
                    else
                    {
                        amount = int.Parse(price);
                    }
                }
                else
                {
                    amount = int.Parse(price);
                }
                sql_cmnd = new SqlCommand("Stp_BasketInsert", cnn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@UserId", SqlDbType.Int).Value = userid;
                sql_cmnd.Parameters.AddWithValue("@BookId", SqlDbType.Int).Value = int.Parse(bookid);
                sql_cmnd.Parameters.AddWithValue("@NetPrice", SqlDbType.BigInt).Value = amount;
                sql_cmnd.Parameters.AddWithValue("@Number", SqlDbType.Int).Value = 1;
                sql_cmnd.ExecuteNonQuery();
                MessageBox.Show(" کتاب انتخابی به سبد خريد اضافه شد");
                cnn.Close();
                totalamount += amount;
                BindBasketGrid();
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

        public void BindBasketGrid()
        {
            string connetionString;
            SqlConnection cnn = null;

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            cnn.Open();

            //SELECT query to fetch data from TBLFREINDS  
            SqlDataAdapter da = new SqlDataAdapter("Select T1.BasketId,T1.UserId,T1.BookId,T2.Bookname,T2.AuthorName,T2.AuthorFamily," +
                                                    "case when T2.AuthorType=1 then \'مولف\' when T2.AuthorType=2 then \'مترجم\' end as AuthorType," +
                                                    "T1.NetPrice,T1.Number From Tbl_UserBasket AS T1 LEFT JOIN Tbl_Books AS T2 ON T1.BookId=T2.BookId " +
                                                    "Where T1.ActionType in (1,2)", cnn);
            //DataSet is virutual database or data container.  
            DataSet ds = new DataSet();
            //Fill data inside ds(DataSet) with TableNamed FriendTable  
            da.Fill(ds, "Tbl_UserBasket");
            //Bind Data with DataGrid control.  
            grdBasket.ItemsSource = ds.Tables["Tbl_UserBasket"].DefaultView;
        }

        private void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            string bookid = "";
            int amount = 0;
            string connetionString;
            SqlConnection cnn = null;
            string sql = "";
            SqlCommand updateCommand;
            DataRowView dataRow = (DataRowView)grdBasket.SelectedItem;
            bookid = dataRow.Row.ItemArray[2].ToString();
            amount = int.Parse(dataRow.Row.ItemArray[7].ToString());

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            sql = "UPDATE Tbl_UserBasket set ActionType = 3 WHERE BookId = " + bookid + " AND UserId = " + userid.ToString();

            try
            {
                cnn.Open();
                updateCommand = new SqlCommand(sql, cnn);
                updateCommand.ExecuteNonQuery();
                MessageBox.Show("کتاب انتخاب شده از سبد خريد حذف شد");
                cnn.Close();
                totalamount -= amount;
                BindBasketGrid();
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

        private void PurchaseValletBt_Click(object sender, RoutedEventArgs e)
        {
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand sql_cmnd, sqlcomm;
            SqlDataReader datareader;
            string sql = "";
            int value = 0;

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            sql = "SELECT VALUE FROM TBL_USERVALLET WHERE USERID='" + userid.ToString()+@"'";

            try
            {
                cnn.Open();
                sqlcomm = new SqlCommand(sql, cnn);
//                sqlcomm.CommandType = CommandType.Text;
                datareader = sqlcomm.ExecuteReader();
                while(datareader.Read())
                {
                    value = int.Parse(datareader.GetValue(0).ToString());
                }
                if (value < totalamount)
                {
                    throw new FormatException("موجودي کيف پول شما براي اين خريد کافي نيست");
                }
                datareader.Close();
                sql_cmnd = new SqlCommand("Stp_UserValletUpdate", cnn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@UserId", SqlDbType.Int).Value = userid;
                sql_cmnd.Parameters.AddWithValue("@Value", SqlDbType.BigInt).Value = totalamount;
                sql_cmnd.Parameters.AddWithValue("@Type", SqlDbType.TinyInt).Value = 2;
                sql_cmnd.ExecuteNonQuery();

                sql_cmnd = new SqlCommand("Stp_PurchaseInsert", cnn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@PUserId", SqlDbType.Int).Value = userid;
                sql_cmnd.ExecuteNonQuery();
                MessageBox.Show("خريد کتب با موفقيت انجام شد");
                cnn.Close();
                BindBasketGrid();
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

        private void PurchaseInternetBt_Click(object sender, RoutedEventArgs e)
        {
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand sql_cmnd;
            string sql = "";

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);

            try
            {
                cnn.Open();
                InternetPurchase ipurchase = new InternetPurchase(username, userfamily, usertype, userid, totalamount, "Basket");
                this.Hide();
                ipurchase.Show();
                sql_cmnd = new SqlCommand("Stp_PurchaseInsert", cnn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@PUserId", SqlDbType.Int).Value = userid;
                sql_cmnd.ExecuteNonQuery();
                //MessageBox.Show("خريد کتب با موفقيت انجام شد");
                cnn.Close();
                BindBasketGrid();
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
