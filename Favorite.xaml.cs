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
    /// Interaction logic for Favorite.xaml
    /// </summary>
    public partial class Favorite : Window
    {
        int userid = 0;
        string bookid = "";
        public Favorite(int userid, string bookid)
        {
            this.userid = userid;
            this.bookid = bookid;
            InitializeComponent();
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            string connetionString;
            SqlConnection cnn = null;
            SqlCommand sql_cmnd;

            connetionString = @"Data Source=SE-PC-7140-1;Initial Catalog=Bookshop;Integrated Security=True";
            cnn = new SqlConnection(connetionString);

            try
            {
                if (Point.Text == "" || int.Parse(Point.Text) > 5 || int.Parse(Point.Text) < 1)
                {
                    throw new FormatException("امتياز را وارد نکرديد و يا خارج از محدوده 1 تا 5 وارد کرديد");
                }
                cnn.Open();
                sql_cmnd = new SqlCommand("Stp_UserFavoritesInsert", cnn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@UserId", SqlDbType.Int).Value = userid;
                sql_cmnd.Parameters.AddWithValue("@BookId", SqlDbType.Int).Value = int.Parse(bookid);
                sql_cmnd.Parameters.AddWithValue("@Point", SqlDbType.TinyInt).Value = int.Parse(Point.Text);
                sql_cmnd.ExecuteNonQuery();
                MessageBox.Show("اطلاعات علاقه مندي با موفقيت ثبت شد");
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
