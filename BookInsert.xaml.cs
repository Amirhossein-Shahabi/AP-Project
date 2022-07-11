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
    /// Interaction logic for BookInsert.xaml
    /// </summary>
    public partial class BookInsert : Window
    {
        string username = "";
        string userfamily = "";
        int usertype = 0;
        int userid = 0;
        public BookInsert(string username, string userfamily, int usertype, int userid)
        {
            this.username = username;
            this.userfamily = userfamily;
            this.usertype = usertype;
            this.userid = userid;
            InitializeComponent();

            AuthorType.SelectedIndex = 0;
            Vip.IsChecked = false;
            Discount.Text = "0";
            DiscountExpDate.Text = "";
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            Books book = new Books(username, userfamily, usertype, userid);
            this.Close();
            book.Show();
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand sql_cmnd, command;
            string sql = "";
            DateTime date = DateTime.Now.AddYears(10);

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);

            try
            {
                if (BookName.Text == "" || CoverFilePath.Text == "" || PdfFilePath.Text == "" || AuthorName.Text == "" ||
                    AuthorFamily.Text == "" || Price.Text == "" || Description.Text == "" || PrintYear.Text == "" ||
                    StockNumber.Text == "")
                {
                    throw new FormatException("فيلد ها بايد پر شوند");
                }
                cnn.Open();
                if (DiscountExpDate.Text != "")
                {
                    sql = @"SELECT dbo.ShamsiToMilady(" + DiscountExpDate.Text + ")";
                    command = new SqlCommand(sql, cnn);
                    command.CommandType = CommandType.Text;
                    date = (DateTime)command.ExecuteScalar();
                }
                sql_cmnd = new SqlCommand("Stp_BooksInsert", cnn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@Bookname", SqlDbType.NVarChar).Value = BookName.Text;
                sql_cmnd.Parameters.AddWithValue("@CoverFilePath", SqlDbType.NVarChar).Value = CoverFilePath.Text;
                sql_cmnd.Parameters.AddWithValue("@PdfFilePath", SqlDbType.NVarChar).Value = PdfFilePath.Text;
                sql_cmnd.Parameters.AddWithValue("@AuthorName", SqlDbType.NVarChar).Value = AuthorName.Text;
                sql_cmnd.Parameters.AddWithValue("@AuthorFamily", SqlDbType.NVarChar).Value = AuthorFamily.Text;
                sql_cmnd.Parameters.AddWithValue("@AuthorType", SqlDbType.TinyInt).Value = AuthorType.SelectedIndex + 1;
                sql_cmnd.Parameters.AddWithValue("@VipTag", SqlDbType.TinyInt).Value = (Vip.IsChecked == true ? 0 : 1);
                sql_cmnd.Parameters.AddWithValue("@Price", SqlDbType.BigInt).Value = int.Parse(Price.Text);
                sql_cmnd.Parameters.AddWithValue("@Description", SqlDbType.NVarChar).Value = Description.Text;
                sql_cmnd.Parameters.AddWithValue("@PrintYear", SqlDbType.NVarChar).Value = PrintYear.Text;
                sql_cmnd.Parameters.AddWithValue("@DiscountPercent", SqlDbType.Int).Value = int.Parse(Discount.Text);
                sql_cmnd.Parameters.AddWithValue("@DiscountExpdate", SqlDbType.SmallDateTime).Value = date;
                sql_cmnd.Parameters.AddWithValue("@StockNumber", SqlDbType.BigInt).Value = int.Parse(StockNumber.Text);
                sql_cmnd.ExecuteNonQuery();
                MessageBox.Show("اطلاعات کتاب با موفقيت ثبت شد");
                cnn.Close();
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
